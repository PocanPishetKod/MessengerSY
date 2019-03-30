using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessengerSY.Core.Domain;
using MessengerSY.Extensions;
using MessengerSY.Models.Contact;
using MessengerSY.Services.UserProfileService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerSY.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public ContactController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpPost("synccontacts")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SyncContacts(ContactsModel model)
        {
            model.Contacts = ExceptMe(model.Contacts);

            var phoneNumbers = model.Contacts.Select(contact => contact.PhoneNumber);
            var existsUserProfiles = _userProfileService.GetExistsUserProfiles(phoneNumbers);
            await InitializeContacts(model.Contacts, existsUserProfiles);

            await AssociateUserProfileWithContacts();
            
            var contacts = model.Contacts.Where(contact =>
                existsUserProfiles.Select(userProfile => userProfile.PhoneNumber).Contains(contact.PhoneNumber));
            return Ok(new ContactsModel()
            {
                Contacts = contacts
            });
        }

        private IEnumerable<ContactModel> ExceptMe(IEnumerable<ContactModel> contacts)
        {
            var me = contacts.FirstOrDefault(contact => contact.PhoneNumber == User.GetUserProfilePhone());
            if (me != null)
            {
                contacts = contacts.Except(new ContactModel[] { me });
            }

            return contacts;
        }

        private async Task InitializeContacts(IEnumerable<ContactModel> contacts, IEnumerable<UserProfile> existsUserProfiles)
        {
            var userProfileId = User.GetUserProfileId();
            var newContacts = new List<Contact>(contacts.Count());
            foreach (var contact in contacts)
            {
                if (!await _userProfileService.IsContactExists(userProfileId, contact.PhoneNumber))
                {
                    var newContact = new Contact()
                    {
                        PhoneNumber = contact.PhoneNumber,
                        ContactName = contact.ContactName,
                        ContactOwnerId = userProfileId,
                        LinkedUserProfileId = existsUserProfiles
                            .FirstOrDefault(userProfile => userProfile.PhoneNumber == contact.PhoneNumber)?.Id
                    };

                    newContacts.Add(newContact);
                }
            }

            await _userProfileService.AddContacts(newContacts);
        }

        private async Task AssociateUserProfileWithContacts()
        {
            var userProfileId = User.GetUserProfileId();
            var linkedContacts = _userProfileService.GetLinkedUserProfileContacts(User.GetUserProfilePhone(), true);
            if (linkedContacts.Count() > 0)
            {
                foreach (var contact in linkedContacts)
                {
                    contact.LinkedUserProfileId = userProfileId;
                }

                await _userProfileService.UpdateContacts(linkedContacts);
            }
        }

        [HttpGet("getcontacts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUserProfileContacts()
        {
            var contacts = _userProfileService.GetLinkedUserProfileContacts(User.GetUserProfileId(), false);
            return Ok(new ContactsModel()
            {
                Contacts = contacts.Select(contact => new ContactModel()
                {
                    ContactName = contact.ContactName,
                    PhoneNumber = contact.PhoneNumber
                })
            });
        }

        [HttpPost("addcontact")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddContact(ContactModel model)
        {
            if (User.GetUserProfilePhone() != model.PhoneNumber)
            {
                var contactUserProfile = await _userProfileService.GetUserProfileByPhone(model.PhoneNumber);
                if (contactUserProfile != null)
                {
                    if (!await _userProfileService.IsContactExists(User.GetUserProfileId(), model.PhoneNumber))
                    {
                        var contact = new Contact()
                        {
                            ContactName = model.ContactName,
                            PhoneNumber = model.PhoneNumber,
                            ContactOwnerId = User.GetUserProfileId(),
                            LinkedUserProfileId = contactUserProfile.Id
                        };

                        await _userProfileService.AddContact(contact);

                        return Ok();
                    }
                }
                else
                {
                    return NotFound();
                }
            }

            return Conflict();
        }
    }
}
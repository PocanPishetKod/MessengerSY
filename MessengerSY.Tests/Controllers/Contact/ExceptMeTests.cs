using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using MessengerSY.Services.UserProfileService;
using MessengerSY.Controllers;
using MessengerSY.Models.Contact;
using System.Linq;

namespace MessengerSY.Tests.Controllers.Contact
{
    public class ExceptMeTests
    {
        //[Fact]
        //public void Test1()
        //{
        //    var userProfileService = new Mock<IUserProfileService>();
        //    var contactController = new ContactController(userProfileService.Object);

        //    var contacts = new List<ContactModel>() { new ContactModel() { ContactName = "Юрий", PhoneNumber = "+79951970169" }, new ContactModel() { ContactName = "Марина", PhoneNumber = "+79284088610" } };

        //    var result = contactController.ExceptMeProto(contacts).First();

        //    Assert.Equal(result.PhoneNumber, "+79284088610");
        //}
    }
}

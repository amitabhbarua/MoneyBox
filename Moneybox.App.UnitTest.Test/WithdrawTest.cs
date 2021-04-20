using AutoFixture;
using FluentAssertions;
using Moneybox.App.Common;
using Moneybox.App.DataAccess;
using Moneybox.App.Features;
using Moneybox.App.Model;
using Moneybox.App.Services;
using Moq;
using System;
using Xunit;

namespace Moneybox.App.UnitTest.Test
{
    public class WithdrawTest
    {
        /// <summary>
        /// Test insufficient fund.
        /// </summary>
        [Fact]
        public void WithdrawInsuffcientFundTest()
        {
            //Arrange
            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockNotificationService = new Mock<INotificationService>();

            Fixture fixture = new Fixture();
            var account = fixture.Create<Account>();
            account.Balance = 100;
            mockAccountRepository.Setup(repo => repo.GetAccountById(account.Id)).Returns(account);
            var withdrawFeature = new WithdrawMoney(mockAccountRepository.Object, mockNotificationService.Object);

            //Act
            Action act = () => withdrawFeature.Execute(account.Id, 500);

            //Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage(ErrorMessage.InsufficientFundMessage);
        }

        /// <summary>
        /// Test low fund notification.
        /// </summary>
        [Fact]
        public void WithdrawAndNotificationForFundsLow()
        {
            //Arrange
            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockNotificationService = new Mock<INotificationService>();

            Fixture fixture = new Fixture();
            var account = fixture.Create<Account>();
            account.Balance = 1000;
            mockAccountRepository.Setup(repo => repo.GetAccountById(account.Id)).Returns(account);
            var withdrawFeature = new WithdrawMoney(mockAccountRepository.Object, mockNotificationService.Object);

            //Act
            withdrawFeature.Execute(account.Id, 600);

            //Assert
            mockNotificationService.Verify(x => x.NotifyFundsLow(account.User.Email), Times.Once);
        }

        /// <summary>
        /// Test withdraw success.
        /// </summary>
        [Fact]
        public void WithdrawSuccessfully()
        {
            //Arrange
            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockNotificationService = new Mock<INotificationService>();

            Fixture fixture = new Fixture();
            var account = fixture.Create<Account>();
            account.Balance = 1000;
            mockAccountRepository.Setup(repo => repo.GetAccountById(account.Id)).Returns(account);
            var withdrawFeature = new WithdrawMoney(mockAccountRepository.Object, mockNotificationService.Object);

            //Act
            withdrawFeature.Execute(account.Id, 400);

            //Assert
            mockAccountRepository.Verify(x => x.Update(account), Times.Once);
        }
    }
}

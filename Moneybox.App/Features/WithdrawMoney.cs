using Moneybox.App.Common;
using Moneybox.App.DataAccess;
using Moneybox.App.Services;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this._accountRepository = accountRepository;
            this._notificationService = notificationService;
        }

        /// <summary>
        /// Implements withdraw money from user account.
        /// </summary>
        /// <param name="fromAccountId" type="Guid">User Account Id</param>
        /// <param name="amount" type="decimal">amount to withdraw</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Execute(Guid fromAccountId, decimal amount)
        {
            // TODO:
            var from = _accountRepository.GetAccountById(fromAccountId);
            if (from.Balance < amount)
            {
                throw new InvalidOperationException(ErrorMessage.InsufficientFundMessage);
            }

            from.Balance -= amount;
            from.Withdrawn -= amount;

            if (from.Balance < 500m)
            {
                _notificationService.NotifyFundsLow(from.User.Email);
            }

            _accountRepository.Update(from);
        }
    }
}

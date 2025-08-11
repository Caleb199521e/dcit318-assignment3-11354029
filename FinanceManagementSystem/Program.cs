using System;
using System.Collections.Generic;

namespace FinanceManagement
{
    // Record for Transaction
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // Interface for processors
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[BankTransfer] Processed transaction #{transaction.Id}: {transaction.Amount:C} for {transaction.Category}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[MobileMoney] Processed transaction #{transaction.Id}: {transaction.Amount:C} for {transaction.Category}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[CryptoWallet] Processed transaction #{transaction.Id}: {transaction.Amount:C} for {transaction.Category}");
        }
    }

    // Base Account class
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            // Default: deduct the amount
            Balance -= transaction.Amount;
            Console.WriteLine($"Applied transaction #{transaction.Id}. New balance: {Balance:C}");
        }
    }

    // Sealed SavingsAccount
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine($"Transaction #{transaction.Id} for {transaction.Amount:C} failed: Insufficient funds");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction #{transaction.Id} applied. Updated balance: {Balance:C}");
        }
    }

    // FinanceApp
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            // i. Instantiate a SavingsAccount
            var account = new SavingsAccount("SA-1001", 10000m);

            // ii. Create three transactions
            var t1 = new Transaction(1, DateTime.Now, 150.30m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 200.00m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 450.00m, "Entertainment");

            // iii. Processors
            ITransactionProcessor p1 = new MobileMoneyProcessor();
            ITransactionProcessor p2 = new BankTransferProcessor();
            ITransactionProcessor p3 = new CryptoWalletProcessor();

            // Process
            p1.Process(t1);
            p2.Process(t2);
            p3.Process(t3);

            // iv. Apply each transaction to the SavingsAccount
            account.ApplyTransaction(t1);
            account.ApplyTransaction(t2);
            account.ApplyTransaction(t3);

            // v. Add to _transactions
            _transactions.AddRange(new[] { t1, t2, t3 });

            Console.WriteLine("\nAll transactions recorded:");
            foreach (var t in _transactions)
            {
                Console.WriteLine(t);
            }
        }

        public static void Main()
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}

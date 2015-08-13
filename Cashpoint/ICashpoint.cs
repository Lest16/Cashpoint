namespace Cashpoint
{
    public interface ICashpoint
    {
        uint Total { get; }

        uint Count { get; }

        void AddBanknote(uint value, uint countBanknotes);

        void RemoveBanknote(uint value, uint countBanknotes);

        bool CanGrant(uint value);
    }
}

namespace Cashpoint
{
    public interface ICashpoint
    {
        uint Total { get; }

        uint Count { get; }

        void AddBanknote(uint value);

        void RemoveBanknote(uint value);

        bool CanGrant(uint value);
    }
}

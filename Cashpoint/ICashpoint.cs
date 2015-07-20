namespace Cashpoint
{
    public interface ICashpoint
    {
        uint Total { get; }

        uint Count { get; }

        ICashpoint LoadFromXmlFile(string filename);

        void SaveToXmlFile(string filename);

        void AddBanknote(uint value);

        void RemoveBanknote(uint value);

        bool CanGrant(uint value);
    }
}

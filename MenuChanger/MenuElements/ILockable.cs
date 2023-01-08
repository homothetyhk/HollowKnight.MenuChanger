namespace MenuChanger.MenuElements
{
    public interface ILockable
    {
        bool Locked { get; }
        void Lock();
        void Unlock();
    }
}

namespace Neutronium.Core.Test.Helper
{
    public class ReadWriteTestViewModel : ReadOnlyTestViewModel
    {
        private int _ReadWrite;
        public int ReadWrite
        {
            get => _ReadWrite;
            set => Set(ref _ReadWrite, value);
        }
    }
}

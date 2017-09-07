using System;

namespace Neutronium.Core.Test.Helper
{
    public class ClrTypesTestViewModel : ViewModelTestBase
    {
        private Int16 _Int16;
        public Int16 Int16
        {
            get { return _Int16; }
            set { Set(ref _Int16, value); }
        }

        private Int32 _Int32;
        public Int32 Int32
        {
            get { return _Int32; }
            set { Set(ref _Int32, value); }
        }

        private Int64 _Int64;
        public Int64 Int64
        {
            get { return _Int64; }
            set { Set(ref _Int64, value); }
        }

        private UInt16 _Uint16;
        public UInt16 Uint16
        {
            get { return _Uint16; }
            set { Set(ref _Uint16, value); }
        }

        private UInt32 _uint32;
        public UInt32 Uint32
        {
            get { return _uint32; }
            set { Set(ref _uint32, value); }
        }

        private UInt64 _Uint64;
        public UInt64 Uint64
        {
            get { return _Uint64; }
            set { Set(ref _Uint64, value); }
        }

        private decimal _Decimal;
        public decimal Decimal
        {
            get { return _Decimal; }
            set { Set(ref _Decimal, value); }
        }

        private float _Float;
        public float Float
        {
            get { return _Float; }
            set { Set(ref _Float, value); }
        }

        private double _Double;
        public double Double
        {
            get { return _Double; }
            set { Set(ref _Double, value); }
        }

        private byte _Byte;
        public byte Byte
        {
            get { return _Byte; }
            set { Set(ref _Byte, value); }
        }

        private sbyte _Sbyte;
        public sbyte Sbyte
        {
            get { return _Sbyte; }
            set { Set(ref _Sbyte, value); }
        }

        private char _Char;
        public char Char
        {
            get { return _Char; }
            set { Set(ref _Char, value); }
        }
    }
}

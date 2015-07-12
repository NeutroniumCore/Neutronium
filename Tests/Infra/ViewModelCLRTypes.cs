using MVVM.CEFGlue.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVM.CEFGlue.Test
{
    public class ViewModelCLRTypes : ViewModelBase
    {
        private Int16 _int16;
        public Int16 int16 
        {
            get { return _int16; }
            set { Set(ref _int16, value, "int16"); }
        }

        private Int32 _int32;
        public Int32 int32
        {
            get { return _int32; }
            set { Set(ref _int32, value, "int32"); }
        }

        private Int64 _int64;
        public Int64 int64
        {
            get { return _int64; }
            set { Set(ref _int64, value, "int64"); }
        }

        private UInt16 _uint16;
        public UInt16 uint16
        {
            get { return _uint16; }
            set { Set(ref _uint16, value, "uint16"); }
        }

        private UInt32 _uint32;
        public UInt32 uint32
        {
            get { return _uint32; }
            set { Set(ref _uint32, value, "uint32"); }
        }

        private UInt64 _uint64;
        public UInt64 uint64
        {
            get { return _uint64; }
            set { Set(ref _uint64, value, "uint64"); }
        }

        private decimal _decimal;
        public decimal Decimal
        {
            get { return _decimal; }
            set { Set(ref _decimal, value, "Decimal"); }
        }

        private float _float;
        public float Float
        {
            get { return _float; }
            set { Set(ref _float, value, "Float"); }
        }

        private double _double;
        public double Double
        {
            get { return _double; }
            set { Set(ref _double, value, "Double"); }
        }

        private byte _byte;
        public byte Byte
        {
            get { return _byte; }
            set { Set(ref _byte, value, "Byte"); }
        }

        private sbyte _sbyte;
        public sbyte Sbyte
        {
            get { return _sbyte; }
            set { Set(ref _sbyte, value, "Sbyte"); }
        }

        private char _char;
        public char Char
        {
            get { return _char; }
            set { Set(ref _char, value, "Char"); }
        }
    }
}

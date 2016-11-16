using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace GameSharp
{
    partial class GB
    {
        internal class CPU : GBInstance
        {
            private const byte flags_zero = (1 << 7);
            private const byte flags_negative = (1 << 6);
            private const byte flags_halfcarry = (1 << 5);
            private const byte flags_carry = (1 << 4);

            private byte a;
            private byte flags;

            private byte b;
            private byte c;

            private byte d;
            private byte e;

            private byte h;
            private byte l;

            private byte timer;

            private UInt16 sp;
            private UInt16 pc;

            public CPU(GB Instance) : base (Instance)
            {
                base.instance = Instance;
                this.Reset();
            }

            private ushort AF
            {
                get
                {
                    return (ushort)((a << 8) + (flags));
                }
                set
                {
                    a = (byte)((value >> 8) & 0xFF);
                    flags = (byte)(value & 0xFF);
                }
            }

            private ushort BC
            {
                get
                {
                    return (ushort)((b << 8) + (c));
                }
                set
                {
                    b = (byte)((value >> 8) & 0xFF);
                    c = (byte)(value & 0xFF);
                }
            }

            private ushort DE
            {
                get
                {
                    return (ushort)((d << 8) + (e));
                }
                set
                {
                    d = (byte)((value >> 8) & 0xFF);
                    e = (byte)(value & 0xFF);
                }
            }

            private ushort HL
            {
                get
                {
                    return (ushort)((h << 8) + (l));
                }
                set
                {
                    h = (byte)((value >> 8) & 0xFF);
                    l = (byte)(value & 0xFF);
                }
            }

            private ushort SP
            {
                get { return sp; }
                set { sp = value; }
            }

            private ushort PC
            {
                get { return pc; }
                set
                {
                    pc = (ushort)(value & 0xFFFF);
                    if (mmu.InBios && value >= 100)
                        mmu.InBios = false;
                }
            }

            public void Reset()
            {
                mmu.InBios = true;
                a = 0x01;
                flags = 0xB0;
                b = 0;
                c = 0x13;
                d = 0;
                e = 0xD8;
                h = 0x01;
                l = 0x4D;

                SP = 0xFFFE;
                PC = 0x100;
            }

            private bool FlagIsSet(Flags flag)
            {
                return (flags & ((byte)flag)) == 0 ? false : true;
            }

            private void FlagSet(Flags flag)
            {
                flags |= (byte)flag;
            }

            private void FlagClear(Flags flag)
            {
                flags &= (byte)~flag;
            }

            private void executeInstruction(ushort opcode)
            {
                switch (opcode)
                {
                    // 0x00
                    case 0x00: this.nop(); break;
                    case 0x01: BC = mmu.GetShort(PC + 1); break;
                    case 0x02: mmu.WriteByte(BC, a); break;
                    case 0x03: BC = inc(BC); break;
                    case 0x04: inc(ref b); break;
                    case 0x05: b = dec(b); break;
                    case 0x06: b = mmu.GetByte(PC + 1); break;
                    case 0x07: a = rlca(a); break;
                    case 0x08: mmu.WriteShort(mmu.GetShort(PC + 1), SP); break;
                    case 0x09: HL = this.add(HL, BC); break;
                    case 0x0A: a = mmu.GetByte(BC); break;
                    case 0x0B: BC = dec(BC); break;
                    case 0x0C: inc(ref c); break;
                    case 0x0D: dec(ref c); break;
                    case 0x0E: c = mmu.GetByte(PC + 1); break;
                    case 0x0F: a = rrca(a); break;

                    // 0x10
                    case 0x10: stop(); break;
                    case 0x11: DE = mmu.GetByte(PC + 1); break;
                    case 0x12: mmu.WriteByte(BC, a); break;
                    case 0x13: DE = inc(DE); break;
                    case 0x14: inc(ref d); break;
                    case 0x15: dec(ref d); break;
                    case 0x16: d = mmu.GetByte(PC + 1); break;
                    case 0x17: a = rla(a); break;
                    case 0x18: PC = (ushort)(PC + mmu.GetByte(PC + 1)); break;
                    case 0x19: HL = this.add(HL, DE); break;
                    case 0x1A: a = mmu.GetByte(BC); break;
                    case 0x1B: BC = dec(BC); break;
                    case 0x1C: inc(ref e); break;
                    case 0x1D: dec(ref e); break;
                    case 0x1E: e = mmu.GetByte(PC + 1); break;
                    case 0x1F: a = rra(a); break;

                    // 0x20
                    case 0x20: jr(!FlagIsSet(Flags.Zero), mmu.GetByte(PC + 1)); break;
                    case 0x21: HL = mmu.GetShort(PC + 1); break;
                    case 0x22: mmu.WriteByte(HL, mmu.GetByte(PC + 1)); HL++; break;
                    case 0x23: HL = inc(HL); break;
                    case 0x24: inc(ref h); break;
                    case 0x25: dec(ref h); break;
                    case 0x26: h = mmu.GetByte(PC + 1); break;
                    case 0x27: daa(); break;
                    case 0x28: jr(FlagIsSet(Flags.Zero), mmu.GetByte(PC + 1)); break;
                    case 0x29: HL = add(HL, HL); break;
                    case 0x2A: a = mmu.GetByte(HL); HL++; break;
                    case 0x2B: HL = dec(HL); break;
                    case 0x2C: inc(ref l); break;
                    case 0x2D: dec(ref l); break;
                    case 0x2E: l = mmu.GetByte(PC + 1); break;
                    case 0x2F: cpl(); break;

                    // 0x30
                    case 0x30: jr(!FlagIsSet(Flags.Carry), mmu.GetByte(PC + 1)); break;
                    case 0x31: SP = mmu.GetShort(PC + 1); break;
                    case 0x32: mmu.WriteByte(HL, mmu.GetByte(PC + 1)); HL--; break;
                    case 0x33: SP = inc(SP); break;
                    case 0x34: mmu.WriteByte(HL, inc(mmu.GetByte(HL))); break;
                    case 0x35: mmu.WriteByte(HL, dec(mmu.GetByte(HL))); break;
                    case 0x36: mmu.WriteByte(HL, mmu.GetByte(PC + 1)); break;
                    case 0x37: scf(); break;
                    case 0x38: jr(FlagIsSet(Flags.Carry), mmu.GetByte(PC + 1)); break;
                    case 0x39: HL = add(HL, SP); break;
                    case 0x3A: a = mmu.GetByte(HL); HL--; break;
                    case 0x3B: SP--; break;
                    case 0x3C: inc(ref a); break;
                    case 0x3D: dec(ref a); break;
                    case 0x3E: a = mmu.GetByte(PC + 1); break;
                    case 0x3F: ccf(); break;

                    //0x40
                    case 0x40: b = b; break;
                    case 0x41: b = c; break;
                    case 0x42: b = d; break;
                    case 0x43: b = e; break;
                    case 0x44: b = h; break;
                    case 0x45: b = l; break;
                    case 0x46: b = mmu.GetByte(HL); break;
                    case 0x47: b = a; break;
                    case 0x48: c = b; break;
                    case 0x49: c = c; break;
                    case 0x4A: c = d; break;
                    case 0x4B: c = e; break;
                    case 0x4C: c = h; break;
                    case 0x4D: c = l; break;
                    case 0x4E: c = mmu.GetByte(HL); break;
                    case 0x4F: c = a; break;

                    // 0x50
                    case 0x50: d = b; break;
                    case 0x51: d = c; break;
                    case 0x52: d = d; break;
                    case 0x53: d = e; break;
                    case 0x54: d = h; break;
                    case 0x55: d = l; break;
                    case 0x56: d = mmu.GetByte(HL); break;
                    case 0x57: d = a; break;
                    case 0x58: e = b; break;
                    case 0x59: e = c; break;
                    case 0x5A: e = d; break;
                    case 0x5B: e = e; break;
                    case 0x5C: e = h; break;
                    case 0x5D: e = l; break;
                    case 0x5E: e = mmu.GetByte(HL); break;
                    case 0x5F: e = a; break;

                    // 0x60
                    case 0x60: h = b; break;
                    case 0x61: h = c; break;
                    case 0x62: h = d; break;
                    case 0x63: h = e; break;
                    case 0x64: h = h; break;
                    case 0x65: h = l; break;
                    case 0x66: h = mmu.GetByte(HL); break;
                    case 0x67: h = a; break;
                    case 0x68: l = b; break;
                    case 0x69: l = c; break;
                    case 0x6A: l = d; break;
                    case 0x6B: l = e; break;
                    case 0x6C: l = h; break;
                    case 0x6D: l = l; break;
                    case 0x6E: l = mmu.GetByte(HL); break;
                    case 0x6F: l = a; break;

                    // 0x70
                    case 0x70: mmu.WriteByte(HL, b); break;
                    case 0x71: mmu.WriteByte(HL, c); break;
                    case 0x72: mmu.WriteByte(HL, d); break;
                    case 0x73: mmu.WriteByte(HL, e); break;
                    case 0x74: mmu.WriteByte(HL, h); break;
                    case 0x75: mmu.WriteByte(HL, l); break;
                    case 0x76: halt(); break;
                    case 0x77: mmu.WriteByte(HL, a); break;
                    case 0x78: a = b; break;
                    case 0x79: a = c; break;
                    case 0x7A: a = d; break;
                    case 0x7B: a = e; break;
                    case 0x7C: a = h; break;
                    case 0x7D: a = l; break;
                    case 0x7E: a = mmu.GetByte(HL); break;
                    case 0x7F: a = a; break;

                    // 0x80
                    case 0x80: add(ref a, b); break;
                    case 0x81: add(ref a, c); break;
                    case 0x82: add(ref a, d); break;
                    case 0x83: add(ref a, e); break;
                    case 0x84: add(ref a, h); break;
                    case 0x85: add(ref a, l); break;
                    case 0x86: add(ref a, mmu.GetByte(HL)); break;
                    case 0x87: add(ref a, a); break;
                    case 0x88: adc(ref a, b); break;
                    case 0x89: adc(ref a, c); break;
                    case 0x8A: adc(ref a, d); break;
                    case 0x8B: adc(ref a, e); break;
                    case 0x8C: adc(ref a, h); break;
                    case 0x8D: adc(ref a, l); break;
                    case 0x8E: adc(ref a, mmu.GetByte(HL)); break;
                    case 0x8F: adc(ref a, a); break;

                    // 0x90
                    case 0x90: sub(ref a, b); break;
                    case 0x91: sub(ref a, c); break;
                    case 0x92: sub(ref a, d); break;
                    case 0x93: sub(ref a, e); break;
                    case 0x94: sub(ref a, h); break;
                    case 0x95: sub(ref a, l); break;
                    case 0x96: sub(ref a, mmu.GetByte(HL)); break;
                    case 0x97: sub(ref a, a); break;
                    case 0x98: sbc(ref a, b); break;
                    case 0x99: sbc(ref a, c); break;
                    case 0x9A: sbc(ref a, d); break;
                    case 0x9B: sbc(ref a, e); break;
                    case 0x9C: sbc(ref a, h); break;
                    case 0x9D: sbc(ref a, l); break;
                    case 0x9E: sbc(ref a, mmu.GetByte(HL)); break;
                    case 0x9F: sbc(ref a, a); break;

                    // 0xA0
                    case 0xA0: and(ref a, b); break;
                    case 0xA1: and(ref a, c); break;
                    case 0xA2: and(ref a, d); break;
                    case 0xA3: and(ref a, e); break;
                    case 0xA4: and(ref a, h); break;
                    case 0xA5: and(ref a, l); break;
                    case 0xA6: and(ref a, mmu.GetByte(HL)); break;
                    case 0xA7: and(ref a, a); break;
                    case 0xA8: xor(ref a, b); break;
                    case 0xA9: xor(ref a, c); break;
                    case 0xAA: xor(ref a, d); break;
                    case 0xAB: xor(ref a, e); break;
                    case 0xAC: xor(ref a, h); break;
                    case 0xAD: xor(ref a, l); break;
                    case 0xAE: xor(ref a, mmu.GetByte(HL)); break;
                    case 0xAF: xor(ref a, a); break;

                    // 0xB0
                    case 0xB0: or(ref a, b); break;
                    case 0xB1: or(ref a, c); break;
                    case 0xB2: or(ref a, d); break;
                    case 0xB3: or(ref a, e); break;
                    case 0xB4: or(ref a, h); break;
                    case 0xB5: or(ref a, l); break;
                    case 0xB6: or(ref a, mmu.GetByte(HL)); break;
                    case 0xB7: or(ref a, a); break;
                    case 0xB8: cp(a, b); break;
                    case 0xB9: cp(a, c); break;
                    case 0xBA: cp(a, d); break;
                    case 0xBB: cp(a, e); break;
                    case 0xBC: cp(a, h); break;
                    case 0xBD: cp(a, l); break;
                    case 0xBE: cp(a, mmu.GetByte(HL)); break;
                    case 0xBF: cp(a, a); break;

                    // 0xC0
                    case 0xC0: ret(!FlagIsSet(Flags.Carry)); break;
                    case 0xC1: BC = pop(); break;
                    case 0xC2: jp(!FlagIsSet(Flags.Zero), mmu.GetShort(PC + 1)); break;
                    case 0xC3: jp(mmu.GetShort(PC + 1)); break;
                    case 0xC4: call(!FlagIsSet(Flags.Zero), mmu.GetShort(PC + 1)); break;
                    case 0xC5: push(BC); break;
                    case 0xC6: a = add(a, mmu.GetByte(PC + 1)); break;
                    case 0xC7: rst(0x00); break;
                    case 0xC8: ret(FlagIsSet(Flags.Zero)); break;
                    case 0xC9: ret(); break;
                    case 0xCA: jp(FlagIsSet(Flags.Zero), mmu.GetShort(PC + 1)); break;
                    //case 0xCB: break; // PREFIX CB
                    case 0xCC: call(FlagIsSet(Flags.Zero), mmu.GetShort(PC + 1)); break;
                    case 0xCD: call(mmu.GetShort(PC + 1)); break;
                    case 0xCE: adc(ref a, mmu.GetByte(PC + 1)); break;
                    case 0xCF: rst(0x08); break;

                    // 0xD0
                    case 0xD0: ret(!FlagIsSet(Flags.Carry)); break;
                    case 0xD1: DE = pop(); break;
                    case 0xD2: jp(!FlagIsSet(Flags.Carry), mmu.GetByte(PC + 1)); break;
                    case 0xD3: nop(); break;
                    case 0xD4: call(!FlagIsSet(Flags.Carry), mmu.GetByte(PC + 1)); break;
                    case 0xD5: push(DE); break;
                    case 0xD6: sub(ref a, mmu.GetByte(PC + 1)); break;
                    case 0xD7: rst(0x10); break;
                    case 0xD8: ret(FlagIsSet(Flags.Carry)); break;
                    case 0xD9: reti(); break;
                    case 0xDA: jp(FlagIsSet(Flags.Carry), mmu.GetShort(PC + 1)); break;
                    case 0xDB: nop(); break;
                    case 0xDC: call(FlagIsSet(Flags.Carry), mmu.GetShort(PC + 1)); break;
                    case 0xDD: nop(); break;
                    case 0xDE: sbc(ref a, mmu.GetByte(PC + 1)); break;
                    case 0xDF: rst(0x18); break;

                    // 0xE0
                    case 0xE0: mmu.WriteByte(0xFF00 + mmu.GetByte(PC + 1), a); break;
                    case 0xE1: HL = pop(); break;
                    case 0xE2: mmu.WriteByte(0xFF00 + c, a); break;
                    case 0xE3: nop(); break;
                    case 0xE4: nop(); break;
                    case 0xE5: push(HL); break;
                    case 0xE6: a = and(a, mmu.GetByte(PC + 1)); break;
                    case 0xE7: rst(0x20); break;
                    case 0xE8: SP = add(SP, mmu.GetByte(PC + 1)); break;
                    case 0xE9: PC = HL; break;
                    case 0xEA: mmu.WriteByte(mmu.GetShort(PC + 1), a); break;
                    case 0xEB: nop(); break;
                    case 0xEC: nop(); break;
                    case 0xED: nop(); break;
                    case 0xEE: a = xor(a, mmu.GetByte(PC + 1)); break;
                    case 0xEF: rst(0x28); break;

                    // 0xF0
                    case 0xF0: a = mmu.GetByte(0xFF00 + mmu.GetByte(PC + 1)); break;
                    case 0xF1: AF = pop(); break;
                    case 0xF2: a = mmu.GetByte(0xFF00 + c); break;
                    case 0xF3: di(); break;
                    case 0xF4: nop(); break;
                    case 0xF5: push(AF); break;
                    case 0xF6: a = or(a, mmu.GetByte(PC + 1)); break;
                    case 0xF7: rst(0x30); break;
                    //case 0xF8: break;
                    case 0xF9: SP = HL; break;
                    case 0xFA: a = mmu.GetByte(mmu.GetShort(PC + 1)); break;
                    case 0xFB: ei(); break;
                    case 0xFC: nop(); break;
                    case 0xFD: nop(); break;
                    case 0xFE: cp(a, mmu.GetByte(PC + 1)); break;
                    case 0xFF: rst(38); break;

                    default: throw new NotImplementedException("OpCode" + opcode + "not implemented");
                }
            }





            #region arithmetic
            private byte add(byte val1, byte val2)
            {
                var result = val1 + val2;

                if ((result & 0xFF00) != 0) FlagSet(Flags.Carry);
                else FlagClear(Flags.Carry);

                if ((0xF & val1 + 0xF & val2) > 0xF) FlagSet(Flags.HalfCarry);
                else FlagClear(Flags.HalfCarry);

                if ((result & 0xFF) == 0) FlagSet(Flags.Zero);
                else FlagClear(Flags.Zero);

                FlagClear(Flags.Negative);

                return (byte)result;
            }

            private void add(ref byte val1, byte val2)
            {
                var result = val1 + val2;

                if ((result & 0xFF00) != 0) FlagSet(Flags.Carry);
                else FlagClear(Flags.Carry);

                if ((0xF & val1 + 0xF & val2) > 0xF) FlagSet(Flags.HalfCarry);
                else FlagClear(Flags.HalfCarry);

                if ((result & 0xFF) == 0) FlagSet(Flags.Zero);
                else FlagClear(Flags.Zero);

                FlagClear(Flags.Negative);

                val1 = (byte)result;
            }

            private ushort add(ushort val1, ushort val2)
            {
                var result = val1 + val2;

                if ((result & 0xF0000) != 0) // set if carry from bit 15
                    FlagSet(Flags.Carry);
                else
                    FlagClear(Flags.Carry);

                if ((0xFFF & val1 + 0xFFF & val2) > 0xFFF) // set if carry from bit 11
                    FlagSet(Flags.HalfCarry);
                else
                    FlagClear(Flags.HalfCarry);

                return (ushort)result;
            }

            private void adc(ref byte val1, byte val2)
            {
                if (FlagIsSet(Flags.Carry)) val2++;
                var result = val1 + val2;

                if ((result & 0xFF00) != 0)
                    FlagSet(Flags.Carry);
                else
                    FlagClear(Flags.Carry);

                if ((0xF & val1 + 0xF & val2) > 0xF)
                    FlagSet(Flags.HalfCarry);
                else
                    FlagClear(Flags.HalfCarry);

                if ((result & 0xFF) == 0)
                    FlagSet(Flags.Zero);
                else
                    FlagClear(Flags.Zero);

                FlagClear(Flags.Negative);

                val1 = (byte)result;
            }

            private void sub(ref byte val1, byte val2)
            {
                var result = val1 - val2;

                FlagSet(Flags.Negative);

                if (val2 > val1) FlagSet(Flags.Carry);
                else FlagClear(Flags.Carry);

                if ((val2 & 0xF) > (val1 & 0xF)) FlagSet(Flags.HalfCarry);
                else FlagClear(Flags.HalfCarry);

                if ((result & 0xFF) == 0) FlagSet(Flags.Zero);
                else FlagClear(Flags.Zero);

                val1 = (byte)result;
            }

            private void sbc(ref byte val1, byte val2)
            {
                if (FlagIsSet(Flags.Carry))
                    val2++;

                var result = val1 - val2;

                FlagSet(Flags.Negative);

                if (val2 > val1) FlagSet(Flags.Carry);
                else FlagClear(Flags.Carry);

                if ((val2 & 0xF) > (val1 & 0xF)) FlagSet(Flags.HalfCarry);
                else FlagClear(Flags.HalfCarry);

                if ((result & 0xFF) == 0) FlagSet(Flags.Zero);
                else FlagClear(Flags.Zero);

                val1 = (byte)result;
            }

            private byte inc(byte val)
            {
                FlagClear(Flags.Negative);

                if ((val & 0xF) == 0xF) FlagSet(Flags.HalfCarry);
                else FlagClear(Flags.HalfCarry);

                val++;

                if (val == 0) FlagSet(Flags.Zero);
                else FlagClear(Flags.Zero);

                return val;
            }

            private void inc(ref byte val)
            {
                FlagClear(Flags.Negative);

                if ((val & 0xF) == 0xF) FlagSet(Flags.HalfCarry);
                else FlagClear(Flags.HalfCarry);

                val++;

                if (val == 0) FlagSet(Flags.Zero);
                else FlagClear(Flags.Zero);
            }

            private ushort inc(ushort val)
            {
                return (ushort)(val + 1);
            }

            private byte dec(byte val)
            {
                FlagSet(Flags.Negative);

                if ((val & 0x0) == 0x0) FlagSet(Flags.HalfCarry);
                else FlagClear(Flags.HalfCarry);

                val--;

                if (val == 0) FlagSet(Flags.Zero);
                else FlagClear(Flags.Zero);

                return val;
            }

            private void dec(ref byte val)
            {
                FlagSet(Flags.Negative);

                if ((val & 0x0) == 0x0) FlagSet(Flags.HalfCarry);
                else FlagClear(Flags.HalfCarry);

                val--;

                if (val == 0) FlagSet(Flags.Zero);
                else FlagClear(Flags.Zero);
            }

            private ushort dec(ushort val)
            {
                return (ushort)(val - 1);
            }
            #endregion

            #region logic
            private byte and(byte val1, byte val2)
            {
                FlagClear(Flags.Negative | Flags.Carry);
                FlagSet(Flags.HalfCarry);

                var result = val1 & val2;

                if (result == 0) FlagSet(Flags.Zero);
                else FlagClear(Flags.Zero);

                return (byte)result;
            }

            private void and(ref byte val1, byte val2)
            {
                FlagClear(Flags.Negative | Flags.Carry);
                FlagSet(Flags.HalfCarry);

                val1 = (byte)(val1 & val2);

                if (val1 == 0) FlagSet(Flags.Zero);
                else FlagClear(Flags.Zero);
            }

            private byte xor(byte val1, byte val2)
            {
                FlagClear(Flags.Carry | Flags.HalfCarry | Flags.Negative);
                var result = val1 ^ val2;

                if (result == 0) FlagSet(Flags.Zero);
                else FlagClear(Flags.Zero);

                return (byte)result;
            }

            private void xor(ref byte val1, byte val2)
            {
                FlagClear(Flags.Carry | Flags.HalfCarry | Flags.Negative);
                val1 = (byte)(val1 ^ val2);

                if (val1 == 0) FlagSet(Flags.Zero);
                else FlagClear(Flags.Zero);
            }

            private byte or(byte val1, byte val2)
            {
                FlagClear(Flags.Carry | Flags.HalfCarry | Flags.Negative);
                var result = val1 | val2;

                if (result == 0) FlagSet(Flags.Zero);
                else FlagClear(Flags.Zero);

                return (byte)result;
            }

            private void or(ref byte val1, byte val2)
            {
                FlagClear(Flags.Carry | Flags.HalfCarry | Flags.Negative);
                val1 = (byte)(val1 | val2);

                if (val1 == 0) FlagSet(Flags.Zero);
                else FlagClear(Flags.Zero);
            }

            private void cp(byte val1, byte val2)
            {
                FlagSet(Flags.Negative);

                if (val1 == val2) FlagSet(Flags.Zero);
                else FlagClear(Flags.Zero);

                if (val1 < val2) FlagSet(Flags.Carry);
                else FlagClear(Flags.Carry);

                if ((val1 & 0xF) < (val2 & 0xF)) FlagSet(Flags.HalfCarry);
                else FlagClear(Flags.HalfCarry);
            }
            #endregion

            #region 16-bit Loads
            private void push(ushort val)
            {
                mmu.WriteByte(SP - 1,(byte)(val >> 8));
                mmu.WriteByte(SP - 2, (byte)val);

                SP -= 2;
            }

            private ushort pop()
            {
                byte low = mmu.GetByte(SP);
                byte high = mmu.GetByte(SP + 1);

                SP += 2;
                return (ushort)((high << 8) + low);
            }
            #endregion

            #region returns
            private void ret(bool flag)
            {
                if (flag) ret();
            }

            private void ret()
            {
                PC = pop();
            }

            private void reti()
            {
                ret();
                ei();
            }
            #endregion

            #region jumps
            private void jp(ushort addr)
            {
                PC = addr;
            }

            private void jp(bool flag, ushort addr)
            {
                if (flag) PC = addr;
            }

            private void jr(byte offset)
            {
                PC += offset;
            }

            private void jr(bool flag, byte offset)
            {
                if (flag) jr(offset);
            }
            #endregion

            private void call(bool flag, ushort addr)
            {
                if (flag) call(addr);
            }

            private void call(ushort addr)
            {
                throw new NotImplementedException();
            }
            
            private byte rlca(byte val)
            {
                FlagClear(Flags.HalfCarry | Flags.Negative | Flags.Zero);
                var temp = val;

                val = (byte)((val << 1) + (FlagIsSet(Flags.Carry) ? 1 : 0));

                if ((temp & 0x80) != 0) FlagSet(Flags.Carry);
                else FlagClear(Flags.Carry);

                
                return val;
            }

            private byte rla(byte val)
            {
                FlagClear(Flags.HalfCarry | Flags.Negative | Flags.Zero);

                var temp = val << 1 + (FlagIsSet(Flags.Carry) ? 1 : 0);

                byte top = (byte)(val >> 7);

                if (top != 0) FlagSet(Flags.Carry);
                else FlagClear(Flags.Carry);

                return (byte)temp;
            }

            private byte rrca(byte val)
            {
                FlagClear(Flags.HalfCarry | Flags.Negative | Flags.Zero);

                if ((val & 0x01) != 0) FlagSet(Flags.Carry);
                else FlagClear(Flags.Carry);

                val = (byte)((val >> 1) + (FlagIsSet(Flags.Carry) ? 0x80 : 0));
                return val;
            }

            private byte rra(byte val)
            {
                FlagClear(Flags.HalfCarry | Flags.Negative | Flags.Zero);

                var low = (byte)(val & 0x01);

                val = (byte)(val >> 1 + (FlagIsSet(Flags.Carry) ? 1 << 7 : 0));

                if (low != 0) FlagSet(Flags.Carry);
                else FlagClear(Flags.Carry);

                return val;
            }

            #region restarts
            private void rst(byte addr)
            {
                push(PC);
                PC = addr;
            }
            #endregion

            #region misc
            private byte swap(byte val)
            {
                var a = (val << 4) & 0xF0;
                var b = (val >> 4) & 0x0F;

                return (byte)(a + b);
            }

            private void daa()
            {
                var correction = (byte)0x06;

                if (FlagIsSet(Flags.Negative))
                    correction = (byte)-correction;


                var upper = a >> 4;
                if (upper > 0x09 | FlagIsSet(Flags.Carry)) upper += (correction << 4);

                var lower = a & 0xF;
                if (lower > 0x09 | FlagIsSet(Flags.HalfCarry)) lower += correction;

                a = (byte)(upper + lower);
            }

            private void cpl()
            {
                a = (byte)~a;
                FlagSet(Flags.HalfCarry | Flags.Negative);
            }

            private void ccf()
            {
                if (FlagIsSet(Flags.Carry)) FlagClear(Flags.Carry);
                else FlagSet(Flags.Carry);

                FlagClear(Flags.HalfCarry | Flags.Negative);
            }

            private void scf()
            {
                FlagSet(Flags.Carry);
                FlagClear(Flags.HalfCarry | Flags.Negative);
            }

            private void nop()
            {
            }

            private void halt()
            {
                throw new NotImplementedException();
            }

            private void stop()
            {
                throw new NotImplementedException();
            }

            private void di()
            {
                throw new NotImplementedException();
            }

            private void ei()
            {
                throw new NotImplementedException();
            }
            #endregion



            enum Flags
            {
                Zero = 1 << 7,
                Negative = 1 << 6,
                HalfCarry = 1 << 5,
                Carry = 1 << 4
            }
        }
    }
}

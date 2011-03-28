using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.CDP1802
{
    public enum C1802OpCodes : byte
    {
        Sub0 = 0,
        INC = 1,
        DEC = 2,
        Sub3 = 3,
        LDA = 4,
        STR = 5,
        IRX = 6,
        Sub7 = 7,
        GLO = 8,
        GHI = 9,
        PLO = 10,
        PHI = 11,
        Sub12 = 12,
        SEP = 13,
        SEX = 14,
        Sub15 = 15
    }

    public enum C1802OpCodesSub0 : byte
    {
        IDL = 0,
        IDN = 1,
    }

    public enum C1802OpCodesSub3 : byte
    {
        BR = 0,
        BQ = 1,
        BZ = 2,
        BDF = 3,
        B1 = 4,
        B2 = 5,
        B3 = 6,
        B4 = 7,
        NBR = 8,
        BNQ = 9,
        BNZ = 10,
        BNF = 11,
        BN1 = 12,
        BN2 = 13,
        BN3 = 14,
        BN4 = 15
    }

    public enum C1802OpCodesSub7 : byte
    {
        RET = 0,
        DIS = 1,
        LDXA = 2,
        STXD = 3,
        ADC = 4,
        SDB = 5,
        SHRC = 6,
        SMB = 7,
        SAV = 8,
        MARK = 9,
        REQ = 10,
        SEQ = 11,
        ADCI = 12,
        SDBI = 13,
        SHLC = 14,
        SMBI = 15
    }

    public enum C1802OpCodesSub12 : byte
    {
        LBR = 0,
        LBQ = 1,
        LBZ = 2,
        LBDF = 3,
        NOP = 4,
        LSNQ = 5,
        LSNZ = 6,
        LSNF = 7,
        NLBR = 8,
        LBNQ = 9,
        LBNZ = 10,
        LBNF = 11,
        LSIE = 12,
        LSQ = 13,
        LSZ = 14,
        LSDF = 15
    }

    public enum C1802OpCodesSub15 : byte
    {
        LDX = 0,
        OR = 1,
        AND = 2,
        XOR = 3,
        ADD = 4,
        SD = 5,
        SHR = 6,
        SM = 7,
        LDI = 8,
        ORI = 9,
        ANI = 10,
        XRI = 11,
        ADI = 12,
        SDI = 13,
        SHL = 14,
        SMI = 15
    }
}

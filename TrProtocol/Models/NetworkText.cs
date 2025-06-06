﻿using System.Text;

namespace TrProtocol.Models;

// Token: 0x020001DE RID: 478
public partial class NetworkText
{

    // Token: 0x060016D9 RID: 5849 RVA: 0x0046C570 File Offset: 0x0046A770
    public NetworkText(string text, Mode mode)
    {
        this._text = text;
        this._mode = mode;
    }

    // Token: 0x060016DA RID: 5850 RVA: 0x0046C588 File Offset: 0x0046A788
    public static NetworkText[] ConvertSubstitutionsToNetworkText(object[] substitutions)
    {
        var array = new NetworkText[substitutions.Length];
        for (var i = 0; i < substitutions.Length; i++)
        {
            if (substitutions[i] is not NetworkText networkText)
            {
                networkText = FromLiteral(substitutions[i].ToString());
            }

            array[i] = networkText;
        }
        return array;
    }

    // Token: 0x060016DB RID: 5851 RVA: 0x0046C5CC File Offset: 0x0046A7CC
    public static NetworkText FromFormattable(string text, params object[] substitutions)
    {
        return new NetworkText(text, Mode.Formattable)
        {
            _substitutions = ConvertSubstitutionsToNetworkText(substitutions)
        };
    }

    // Token: 0x060016DC RID: 5852 RVA: 0x0046C5E4 File Offset: 0x0046A7E4
    public static NetworkText FromLiteral(string text)
    {
        return new NetworkText(text, Mode.Literal);
    }

    // Token: 0x060016DD RID: 5853 RVA: 0x0046C5F0 File Offset: 0x0046A7F0
    public static NetworkText FromKey(string key, params object[] substitutions)
    {
        return new NetworkText(key, Mode.LocalizationKey)
        {
            _substitutions = NetworkText.ConvertSubstitutionsToNetworkText(substitutions)
        };
    }

    // Token: 0x060016DE RID: 5854 RVA: 0x0046C608 File Offset: 0x0046A808
    public int GetMaxSerializedSize()
    {
        var num = 0;
        num++;
        num += 4 + Encoding.UTF8.GetByteCount(this._text);
        if (this._mode != Mode.Literal)
        {
            num++;
            for (var i = 0; i < this._substitutions.Length; i++)
            {
                num += this._substitutions[i].GetMaxSerializedSize();
            }
        }
        return num;
    }

    // Token: 0x060016DF RID: 5855 RVA: 0x0046C660 File Offset: 0x0046A860
    public void Serialize(BinaryWriter writer)
    {
        writer.Write((byte) this._mode);
        writer.Write(this._text);
        this.SerializeSubstitutionList(writer);
    }

    // Token: 0x060016E0 RID: 5856 RVA: 0x0046C684 File Offset: 0x0046A884
    public void SerializeSubstitutionList(BinaryWriter writer)
    {
        if (this._mode == Mode.Literal)
        {
            return;
        }
        writer.Write((byte) this._substitutions.Length);
        for (var i = 0; i < (this._substitutions.Length & 255); i++)
        {
            this._substitutions[i].Serialize(writer);
        }
    }

    // Token: 0x060016E1 RID: 5857 RVA: 0x0046C6D0 File Offset: 0x0046A8D0
    public static NetworkText Deserialize(BinaryReader reader)
    {
        var mode = (Mode) reader.ReadByte();
        NetworkText networkText = new(reader.ReadString(), mode);
        networkText.DeserializeSubstitutionList(reader);
        return networkText;
    }

    // Token: 0x060016E2 RID: 5858 RVA: 0x0046C6F8 File Offset: 0x0046A8F8
    public static NetworkText DeserializeLiteral(BinaryReader reader)
    {
        var mode = (Mode) reader.ReadByte();
        NetworkText networkText = new(reader.ReadString(), mode);
        networkText.DeserializeSubstitutionList(reader);
        if (mode != Mode.Literal)
        {
            networkText.SetToEmptyLiteral();
        }
        return networkText;
    }

    // Token: 0x060016E3 RID: 5859 RVA: 0x0046C72C File Offset: 0x0046A92C
    public void DeserializeSubstitutionList(BinaryReader reader)
    {
        if (this._mode == Mode.Literal)
        {
            return;
        }
        this._substitutions = new NetworkText[(reader.ReadByte())];
        for (var i = 0; i < this._substitutions.Length; i++)
        {
            this._substitutions[i] = NetworkText.Deserialize(reader);
        }
    }

    // Token: 0x060016E4 RID: 5860 RVA: 0x0046C774 File Offset: 0x0046A974
    public void SetToEmptyLiteral()
    {
        this._mode = Mode.Literal;
        this._text = string.Empty;
        this._substitutions = null;
    }

    // Token: 0x060016E5 RID: 5861 RVA: 0x0046C790 File Offset: 0x0046A990
    public override string ToString()
    {
        try
        {
            switch (this._mode)
            {
                case Mode.Literal:
                    return this._text;
                case Mode.Formattable:
                {
                    var text = this._text;
                    object[] substitutions = this._substitutions;
                    return string.Format(text, substitutions);
                }
                case Mode.LocalizationKey:
                {
                    var text2 = this._text;
                    return $"lang[{text2}][{string.Join<NetworkText>(',', this._substitutions)}]";
                }
                default:
                    return this._text;
            }
        }
        catch
        {
            this.SetToEmptyLiteral();
        }
        return this._text;
    }

    // Token: 0x060016E6 RID: 5862 RVA: 0x0046C844 File Offset: 0x0046AA44
    public string ToDebugInfoString(string linePrefix = "")
    {
        var text = string.Format("{0}Mode: {1}\n{0}Text: {2}\n", linePrefix, this._mode, this._text);
        if (this._mode == Mode.LocalizationKey)
        {
            text += string.Format("{0}Localized Text: {1}\n", linePrefix, $"lang[{this._text}]");
        }
        if (this._mode != Mode.Literal)
        {
            for (var i = 0; i < this._substitutions.Length; i++)
            {
                text += string.Format("{0}Substitution {1}:\n", linePrefix, i);
                text += this._substitutions[i].ToDebugInfoString(linePrefix + "\t");
            }
        }
        return text;
    }

    // Token: 0x04001489 RID: 5257
    public static readonly NetworkText Empty = NetworkText.FromLiteral("");

    // Token: 0x0400148A RID: 5258
    public NetworkText[] _substitutions;

    // Token: 0x0400148B RID: 5259
    public string _text;

    // Token: 0x0400148C RID: 5260
    public Mode _mode;

    // Token: 0x020001DF RID: 479
    public enum Mode : byte
    {
        // Token: 0x0400148E RID: 5262
        Literal,
        // Token: 0x0400148F RID: 5263
        Formattable,
        // Token: 0x04001490 RID: 5264
        LocalizationKey
    }
}
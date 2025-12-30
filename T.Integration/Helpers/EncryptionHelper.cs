using T.Integration.External;

namespace T.Integration.Helpers;

internal static class EncryptionHelper
{
    static BlowFish? blowFish;
    static byte[]? keyCache;

    public static void Initialize()
    {
        if (string.IsNullOrWhiteSpace(Config.encryptionKey))
        {
            Logger.Warning("Encryption key is empty or whitespace, no encrpytion will be used.");
            return;
        }

        blowFish = new(Config.encryptionKey);
        keyCache = System.Text.Encoding.UTF8.GetBytes(Config.encryptionKey);
    }

    public static void DecryptHeader(List<byte> data)
    {
        if (blowFish == null) return;

        uint num = (uint)((data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3]);
        uint num2 = (uint)((data[4] << 24) | (data[5] << 16) | (data[6] << 8) | data[7]);
        ulong num3 = num;
        num3 = (num3 << 32) + num2;
        blowFish.Decrypt(ref num3);
        num = (uint)(num3 >> 32);
        num2 = (uint)num3;
        data[0] = (byte)(((num & 0xFF000000u) >> 24) & 0xFFu);
        data[1] = (byte)(((num & 0xFF0000) >> 16) & 0xFFu);
        data[2] = (byte)(((num & 0xFF00) >> 8) & 0xFFu);
        data[3] = (byte)(num & 0xFFu & 0xFFu);
        data[4] = (byte)(((num2 & 0xFF000000u) >> 24) & 0xFFu);
        data[5] = (byte)(((num2 & 0xFF0000) >> 16) & 0xFFu);
        data[6] = (byte)(((num2 & 0xFF00) >> 8) & 0xFFu);
        data[7] = (byte)(num2 & 0xFFu & 0xFFu);
    }

    public static void EncryptHeader(List<byte> data)
    {
        if (blowFish == null) return;

        uint num = (uint)((data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3]);
        uint num2 = (uint)((data[4] << 24) | (data[5] << 16) | (data[6] << 8) | data[7]);
        ulong num3 = num;
        num3 = (num3 << 32) + num2;
        blowFish.Encrypt(ref num3);
        num = (uint)(num3 >> 32);
        num2 = (uint)num3;
        data[0] = (byte)(((num & 0xFF000000u) >> 24) & 0xFFu);
        data[1] = (byte)(((num & 0xFF0000) >> 16) & 0xFFu);
        data[2] = (byte)(((num & 0xFF00) >> 8) & 0xFFu);
        data[3] = (byte)(num & 0xFFu & 0xFFu);
        data[4] = (byte)(((num2 & 0xFF000000u) >> 24) & 0xFFu);
        data[5] = (byte)(((num2 & 0xFF0000) >> 16) & 0xFFu);
        data[6] = (byte)(((num2 & 0xFF00) >> 8) & 0xFFu);
        data[7] = (byte)(num2 & 0xFFu & 0xFFu);
    }

    public static byte[] EncryptData(byte[] data)
    {
        if (keyCache == null) return data;

        return XXTEA.Encrypt(data, keyCache);
    }

    public static byte[] DecryptData(byte[] data)
    {
        if (keyCache == null) return data;

        return XXTEA.Decrypt(data, keyCache);
    }
}

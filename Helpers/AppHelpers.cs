using HashidsNet;

namespace CQRSyMediatR.Helpers;
public static class AppHelpers
{
    public const string HashIdsSalt = "s3cret_s4alt";
    public static string ToHashId(this int number) =>
        GetHasher().Encode(number);
    public static int FromHashId(this string encoded) =>
        GetHasher().Decode(encoded).FirstOrDefault();
    private static Hashids GetHasher() => new(HashIdsSalt, 8);
}

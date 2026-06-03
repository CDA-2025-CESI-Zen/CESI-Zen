using System.Security.Cryptography;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Accounts.ValueObjects;
public readonly record struct Password(string Hash) {

    private const int PBKDF2_ITER_COUNT    = 100_000; 
    private const int PBKDF2_SUBKEY_LENGTH = 256/8;
    private const int SALT_SIZE            = 128/8;

    /* =======================
        * HASHED PASSWORD FORMATS
        * =======================
        * 
        * Version 0:
        * PBKDF2 with HMAC-SHA1, 128-bit salt, 256-bit subkey, 1000 iterations.
        * (See also: SDL crypto guidelines v5.1, Part III)
        * Format: { 0x00, salt, subkey }
        */
    public static IResponse<Password> TryCreate(string value) {

        value = value.Trim();
        if (
            string.Concat(value.Where(char.IsLower)).Length < 4 ||
            string.Concat(value.Where(char.IsUpper)).Length < 4 ||
            string.Concat(value.Where(char.IsAsciiDigit)).Length < 4
        ) return Response.Failure<Password>(new InvariantException<Password>("Un mot de passe doit contenir au moins 4 minuscules, 4 majuscules, et 4 chiffres !"));

        // Generate salt
        var salt = new byte[SALT_SIZE];
        RandomNumberGenerator.Fill(salt);

        // Derive subkey
        var subkey = Rfc2898DeriveBytes.Pbkdf2(
            password: value,
            salt: salt,
            iterations: PBKDF2_ITER_COUNT,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: PBKDF2_SUBKEY_LENGTH
        );

        var outputBytes = new byte[1 + SALT_SIZE + PBKDF2_SUBKEY_LENGTH];
        outputBytes[0] = 0x00;
        Buffer.BlockCopy(salt,   0, outputBytes, 1,               SALT_SIZE);
        Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SALT_SIZE,   PBKDF2_SUBKEY_LENGTH);

        return Response.Success(new Password(Convert.ToBase64String(outputBytes)));
    }

    public static Password FromNoise() {
        var random      = new Random();
        var outputBytes = new byte[1 + SALT_SIZE + PBKDF2_SUBKEY_LENGTH];
        random.NextBytes(outputBytes);
        return new Password(Convert.ToBase64String(outputBytes));
    }


    public IResponse TryVerify(string password) {

        var hashedPasswordBytes = Convert.FromBase64String(this.Hash);

        // Verify a version 0 text hash
        if (hashedPasswordBytes.Length != (1 + SALT_SIZE + PBKDF2_SUBKEY_LENGTH) ||
            hashedPasswordBytes[0] != 0x00
        ) return Response.Failure("Mot de passe invalide !");

        var salt = new byte[SALT_SIZE];
        Buffer.BlockCopy(hashedPasswordBytes, 1, salt, 0, SALT_SIZE);

        var storedSubkey = new byte[PBKDF2_SUBKEY_LENGTH];
        Buffer.BlockCopy(hashedPasswordBytes, 1 + SALT_SIZE, storedSubkey, 0, PBKDF2_SUBKEY_LENGTH);

        // Derive subkey
        var generatedSubkey = Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: salt,
            iterations: PBKDF2_ITER_COUNT,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: PBKDF2_SUBKEY_LENGTH
        );

        return CryptographicOperations.FixedTimeEquals(storedSubkey, generatedSubkey)
            ? Response.Success()
            : Response.Failure("Mot de passe incorrect !");
    }
}
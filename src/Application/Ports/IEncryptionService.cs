namespace CesiZen.Application.Ports;
public interface IEncryptionService {
    public string Encrypt(string data);
    public string Decrypt(string encryptedData);
}
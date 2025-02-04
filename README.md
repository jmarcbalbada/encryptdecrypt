# Encrypt-Decrypt using AES

This Windows Forms application allows you to encrypt and decrypt files using AES (Advanced Encryption Standard) with a secure key. You can load files, encrypt or decrypt them, and save the results.

## Features
- AES Encryption: Encrypt files securely using AES with a 32-byte key.
- AES Decryption: Decrypt previously encrypted files back to their original state.
- File Dialog: Select files to encrypt or decrypt using built-in file dialogs.
- Random Initialization Vector (IV): The encryption uses a randomly generated IV for each file, ensuring security.

## Key Management
- Secret Key:
The application uses a predefined AES key for encryption and decryption. The default key is sample123 but can be customized as needed.

- IV (Initialization Vector):
A random 16-byte IV is generated each time a file is encrypted.

## Encryption Process
1. A random IV is generated for each encryption.
2. The file is encrypted with AES-256 (32-byte key).
3. The IV is written at the beginning of the encrypted file.
4. The resulting file is saved with a .enc extension.

### Notes
This application is designed for local file encryption/decryption.
The secret key and IV are hard-coded in this version for simplicity but should be handled securely in production.

All rights reserved 2025.

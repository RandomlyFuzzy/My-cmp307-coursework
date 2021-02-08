using System;
using System.Collections.Generic;
using System.Text;

namespace CodeNameTwang.Services.Interfaces
{
    public interface IAsymmetricEncryption
    {
        string GetPublicKey(string key = "");
        string GetPrivateKey(string key = "");

        string EncodeString(string pubkey,string input);
        string DecodeString(string pvtkey, string output);
    }
}

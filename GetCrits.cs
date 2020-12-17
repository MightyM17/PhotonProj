using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System;
using System.Security.Cryptography;
using System.Linq;

public class GetCrits : MonoBehaviour
{
    private static string hash="logdBF':}kmsjsu!@$2A";
    [SerializeField]public List<LevelCrits> playerPartynew;
    void Start()
    {
        Restore();
        Debug.Log("Checkpp"+playerPartynew);
    }
    void Restore()
    {
        string path=Application.persistentDataPath + "/savefile.biocrits";
        if(File.Exists(path))
        {
            BinaryFormatter formatter=new BinaryFormatter();
            FileStream stream = new FileStream(path,FileMode.Open);
            string x=formatter.Deserialize(stream) as string;
            string p=Decrypt(x);
            if(p!=null&&p.Length>0)
            {
                SaveData s=JsonUtility.FromJson<SaveData> (p);
                if(s!=null)
                {
                    playerPartynew=s.playerParty;
                }
            }
            stream.Close();
        }
        else
        {
            Debug.LogError("No saved data");
            //playerPartynew.crits.Add(defaultcrit);
        }
    }
    public static string Decrypt(string input)
    {
        byte[] data=Convert.FromBase64String(input);
        using(MD5CryptoServiceProvider md5=new MD5CryptoServiceProvider())
        {
            byte[] key=md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            using(TripleDESCryptoServiceProvider trip = new TripleDESCryptoServiceProvider(){Key=key,Mode=CipherMode.ECB,Padding=PaddingMode.PKCS7})
            {
                ICryptoTransform tr=trip.CreateDecryptor();
                byte[] results = tr.TransformFinalBlock(data,0,data.Length);
                return UTF8Encoding.UTF8.GetString(results);
            }
        }
    }
    public LevelCrits GetHealthyCrit4()
    {
        return playerPartynew.Take(4).Where(x => x.HP > 0).FirstOrDefault();
    }
}

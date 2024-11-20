using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileSaveHandler
{
    private string saveDirPath = "";
    private string saveFileName = "";
    private bool useEncyption = false;
    private readonly string encryptionCodeWord = "zorro";

    public FileSaveHandler(string saveDirPath, string saveFileName, bool useEncyption)
    {
        this.saveDirPath = saveDirPath;
        this.saveFileName = saveFileName;
        this.useEncyption = useEncyption;
    }


    //função que vai criar/subistituir o arquivo de save e escrever as informções à serem salvas nele
    public InfosSave Carregar()
    {
        string fullPath = Path.Combine(saveDirPath, saveFileName);

        InfosSave saveCarregado = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string saveParaCarregar = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        saveParaCarregar = reader.ReadToEnd();
                    }
                }

                if (useEncyption)
                {
                    saveParaCarregar = EncrptDecrypt(saveParaCarregar);
                }

                saveCarregado = JsonUtility.FromJson<InfosSave>(saveParaCarregar);
            }
            catch (Exception e)
            {
                Debug.LogError("Erro ao tentar carregar um arquivo de save: " + fullPath + "\n" + e);
            }
        }

        return saveCarregado;
    }

    //função que vai procurar o arquivo de save e ler as informções contida nele
    public void Salvar(InfosSave infosSave)
    {
        string fullPath = Path.Combine(saveDirPath, saveFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string saveToStore = JsonUtility.ToJson(infosSave, true);

            if (useEncyption)
            {
                saveToStore = EncrptDecrypt(saveToStore);
            }

            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(saveToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Erro ao tentar salvar um arquivo de save: " + fullPath + "\n" + e);
        }
    }

    //função para cryptografar o arquivo de save
    private string EncrptDecrypt(string data)
    {
        string modifiedData = "";
        for(int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }

        return modifiedData;
    }
}

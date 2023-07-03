using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class GroupedData
{
    public string NativeId { get; set; }
    public string Name { get; set; }
    public string IdNo { get; set; }
    public List<Dictionary<string, string>> Fields { get; set; }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Veuillez entrer le chemin vers le fichier XML :");
        string inputFilePath = Console.ReadLine();

        // Charger le fichier XML
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(inputFilePath);

        // Créer une liste pour stocker les enregistrements regroupés
        List<GroupedData> groupedDataList = new List<GroupedData>();

        // Récupérer tous les noeuds "data" du fichier XML
        XmlNodeList dataNodes = xmlDoc.SelectNodes("//data");

        // Parcourir chaque noeud "data"
        foreach (XmlNode dataNode in dataNodes)
        {
            string objectId = dataNode.SelectSingleNode("object_id")?.InnerText;
            string elementId = dataNode.SelectSingleNode("element_id")?.InnerText;
            string valueLongText1 = dataNode.SelectSingleNode("value_longtext1")?.InnerText;
            string name = dataNode.SelectSingleNode("name")?.InnerText;
            string idNo = dataNode.SelectSingleNode("idno")?.InnerText;
            // Vérifier si un enregistrement avec le même objectId existe déjà dans la liste
            GroupedData existingRecord = groupedDataList.Find(record => record.NativeId == objectId);

            if (existingRecord == null)
            {
                // Créer un nouvel enregistrement s'il n'existe pas déjà
                GroupedData newRecord = new GroupedData
                {
                    NativeId = objectId,
                    Name = name,
                    IdNo = idNo,
                    Fields = new List<Dictionary<string, string>>()
                };

                // Créer le dictionnaire de champs pour elementId et valueLongText1
                Dictionary<string, string> fieldDictionary = new Dictionary<string, string>();
                fieldDictionary.Add(elementId, valueLongText1);

                // Ajouter le dictionnaire de champs à l'enregistrement
                newRecord.Fields.Add(fieldDictionary);

                // Ajouter le nouvel enregistrement à la liste
                groupedDataList.Add(newRecord);
            }
            else
            {
                // Créer le dictionnaire de champs pour elementId et valueLongText1
                Dictionary<string, string> fieldDictionary = new Dictionary<string, string>();
                fieldDictionary.Add(elementId, valueLongText1);

                // Ajouter le dictionnaire de champs à l'enregistrement existant
                existingRecord.Fields.Add(fieldDictionary);
            }
        }

        // Créer un nouveau document XML contenant les enregistrements regroupés
        XmlDocument outputXmlDoc = new XmlDocument();

        // Créer le nœud racine "records"
        XmlElement recordsNode = outputXmlDoc.CreateElement("records");
        outputXmlDoc.AppendChild(recordsNode);

        // Créer un nœud "record" pour chaque enregistrement regroupé
        foreach (GroupedData groupedData in groupedDataList)
        {
            XmlElement recordNode = outputXmlDoc.CreateElement("record");

            // Créer le champ "native_id" avec la valeur objectId
            XmlElement nativeIdField = outputXmlDoc.CreateElement("field");
            nativeIdField.SetAttribute("id", "native_id");
            nativeIdField.InnerText = groupedData.NativeId;
            recordNode.AppendChild(nativeIdField);
            // Créer le champ "name" avec la valeur Name
            XmlElement nameField = outputXmlDoc.CreateElement("field");
            nameField.SetAttribute("id", "name");
            nameField.InnerText = groupedData.Name;
            recordNode.AppendChild(nameField);

            // Créer le champ "idno" avec la valeur IdNo
            XmlElement idNoField = outputXmlDoc.CreateElement("field");
            idNoField.SetAttribute("id", "idno");
            idNoField.InnerText = groupedData.IdNo;
            recordNode.AppendChild(idNoField);
            //
            // Créer les champs "field" correspondant à elementId et valueLongText1
            foreach (Dictionary<string, string> fieldDictionary in groupedData.Fields)
            {
                foreach (KeyValuePair<string, string> field in fieldDictionary)
                {
                    XmlElement fieldNode = outputXmlDoc.CreateElement("field");
                    fieldNode.SetAttribute("id", field.Key);
                    fieldNode.InnerText = field.Value;
                    recordNode.AppendChild(fieldNode);
                }
            }

            recordsNode.AppendChild(recordNode);
        }

        // Sauvegarder le document XML regroupé dans un fichier
        Console.WriteLine("Veuillez entrer le chemin vers le fichier de sortie XML :");
        string outputFilePath = Console.ReadLine();
        outputXmlDoc.Save(outputFilePath);

        Console.WriteLine("Le fichier XML regroupé a été créé avec succès !");
    }
}

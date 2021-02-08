using CodeNameTwang.Model;
using CodeNameTwang.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace CodeNameTwang.Services.RestAPI
{
    public class JsonParser
    {
        public static object Convert(string value, out Type type)
        {
            //int 
            if (Regex.Match(value, "^(-)?\\d+$").Success)
            {
                type = typeof(int);
                return int.Parse(value);
            }
            //float
            if (Regex.Match(value, "^[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?$").Success)
            {
                type = typeof(float);
                return float.Parse(value);
            }
            //datetime
            if (Regex.Match(value, "^\\d\\d\\d\\d-([0-1]\\d|\\d)-([0-3]\\d|\\d)(T| )([0-2]\\d):([0-5]\\d):(\\d\\d\\d|\\d\\d)($|.\\d\\d\\d(Z|$))").Success)
            {
                type = typeof(DateTime);
                return DateTime.Parse(value.Replace("T", " "));
            }
            //bool
            if (Regex.Match(value.ToLower(), "(true|false)").Success)
            {
                type = typeof(bool);
                return bool.Parse(value);
            }
            //string
            type = typeof(string);
            return value ?? " ";
        }

        public static List<T> Parse<T>(string Json) where T : JsonObject
        {
            byte[] data = Encoding.UTF8.GetBytes(Json);
            Utf8JsonReader reader = new Utf8JsonReader(data, isFinalBlock: true, state: default);
            List<T> lst = new List<T>();
            T Set = Activator.CreateInstance(typeof(T)) as T;
            string property = "";
            while (reader.Read())
            {
                //Console.Write(reader.TokenType);

                switch (reader.TokenType)
                {
                    case JsonTokenType.EndObject:
                        lst.Add(Set);
                        Set = Activator.CreateInstance(typeof(T)) as T;
                        break;
                    case JsonTokenType.StartObject:
                        Set = Activator.CreateInstance(typeof(T)) as T;
                        break;
                    case JsonTokenType.PropertyName:
                        property = reader.GetString();
                        break;
                    case JsonTokenType.Number:
                        Type opttype = typeof(int);
                        if (property == "duration")
                        {
                            var a = 01;
                        }
                        object output = Convert(""+reader.GetDouble(), out opttype);
                        typeof(T)
                            .GetMethod("SetProperty")
                            .MakeGenericMethod(opttype)
                            .Invoke(Set, new object[] {
                            property,
                            output,
                            true });
                        break;
                    case JsonTokenType.String:
                        opttype = typeof(int);
                        if (property == "duration") {
                            var a = 01;
                        }
                        output = Convert(reader.GetString(), out opttype);
                        typeof(T)
                            .GetMethod("SetProperty")
                            .MakeGenericMethod(opttype)
                            .Invoke(Set, new object[] {
                            property,
                            output,
                            true });
                        break;
                    case JsonTokenType.True:
                    case JsonTokenType.False:
                        typeof(T)
                            .GetMethod("SetProperty")
                            .MakeGenericMethod(typeof(bool))
                            .Invoke(Set, new object[] {
                            property,
                            reader.GetBoolean(),
                            true });
                        break;
                        
                    case JsonTokenType.StartArray:
                    case JsonTokenType.EndArray:
                    case JsonTokenType.Null:
                    default:
                        Console.WriteLine("invalid parse of " + reader.TokenType);
                        break;
                }
            }
            return lst;
        }
    }
}

﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project_Radon.Helpers
{
    public static class Json
    {
        public static object locker = new object();
        public static async Task<T> ToObjectAsync<T>(string value)
        {

            return await Task.Run<T>(() =>
            {
                lock(locker)
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
                
            });
        }

        public static async Task<string> StringifyAsync(object value)
        {
            return await Task.Run<string>(() =>
            {
                lock(locker)
                {
                    return JsonConvert.SerializeObject(value);
                }
                
            });
        }
    }
}

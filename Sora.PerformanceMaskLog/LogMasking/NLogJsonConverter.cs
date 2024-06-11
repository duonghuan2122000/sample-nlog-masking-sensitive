using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sora.PerformanceMaskLog.LogMasking
{
    public class NLogJsonConverter : IJsonConverter
    {
        private readonly IConfiguration _configuration;

        public NLogJsonConverter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SerializeObject(object value, StringBuilder builder)
        {
            var result = Newtonsoft.Json.JsonConvert.SerializeObject(value, Formatting.None, new JsonSerializerSettings
            {
                ContractResolver = new ShouldSerializeContractResolver(_configuration)
            });
            builder.Append(result);
            return true;
        }
    }

    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        private static readonly string StarCharn = "***";

        private List<string> SensitivePropNames;

        public ShouldSerializeContractResolver(IConfiguration configuration)
        {
            SensitivePropNames = [.. configuration.GetValue<string>("SensitivePropNames").Split(",")];
        }

        protected override JsonProperty CreateProperty(MemberInfo member,
                MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            IValueProvider valueProvider = base.CreateMemberValueProvider(member);
            if (!string.IsNullOrEmpty(property.PropertyName)
                && SensitivePropNames.Contains(property.PropertyName, StringComparer.OrdinalIgnoreCase))
            {
                property.ValueProvider = new SensitiveValueProvider(valueProvider, StarCharn);
            }
            return property;
        }
    }

    public class SensitiveValueProvider : IValueProvider
    {
        private IValueProvider _provider;

        private string _sensitiveData;

        public SensitiveValueProvider(IValueProvider provider, string sensitiveData)
        {
            _provider = provider;
            _sensitiveData = sensitiveData;
        }

        public object GetValue(object target)
        {
            if (_provider.GetValue(target) == null || string.IsNullOrEmpty(_provider.GetValue(target).ToString()))
            {
                return string.Empty;
            }
            return _sensitiveData;
        }

        public void SetValue(object target, object value)
        {
            throw new System.NotImplementedException();
        }
    }
}
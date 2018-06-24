﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Paillave.Etl.Core.Helpers
{
    public class LineIndexParserConfiguration<TDest> where TDest : new()
    {
        protected Dictionary<int, PropertyMap> IndexToPropertyDictionary { get; } = new Dictionary<int, PropertyMap>();
        protected CultureInfo CultureInfo { get; private set; }
        public LineIndexParserConfiguration<TDest> MapColumnToProperty<TField>(int index, Expression<Func<TDest, TField>> memberLamda, CultureInfo cultureInfo = null)
        {
            PropertyInfo propertyInfo = this.GetPropertyInfo(memberLamda);
            this.IndexToPropertyDictionary[index] = new PropertyMap
            {
                PropertyInfo = propertyInfo,
                TypeConverter = TypeDescriptor.GetConverter(propertyInfo.PropertyType),
                CultureInfo = cultureInfo
            };
            return this;
        }
        public LineIndexParserConfiguration<TDest> WithGlobalCultureInfo(CultureInfo cultureInfo)
        {
            this.CultureInfo = cultureInfo;
            return this;
        }
        private PropertyInfo GetPropertyInfo<TField>(Expression<Func<TDest, TField>> memberLamda)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression != null)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null) return property;
            }
            throw new ArgumentException("Not a navigation expression", nameof(memberLamda));
        }
        public virtual ILineProcessor<TDest> GetLineProcessor()
        {
            return new LineProcessor<TDest>(this.IndexToPropertyDictionary, this.CultureInfo);
        }
    }
}

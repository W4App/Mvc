// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc
{
    internal class TypeFilterFactory : Attribute, IFilterFactory, IOrderedFilter
    {
        private IFilterFactory _filterFactory;

        public TypeFilterFactory(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(IFilterFactory).IsAssignableFrom(type))
            {
                var message = Resources.FormatTypeMustDeriveFromType(
                   type.FullName,
                   typeof(IFilterFactory).FullName);
                throw new ArgumentException(message, nameof(type));
            }

            ImplementationType = type;
        }

        public Type ImplementationType { get; }

        public int Order { get; set; }

        public bool IsReusable { get; set; }

        /// <inheritdoc />
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (_filterFactory == null)
            {
                _filterFactory = (IFilterFactory)ActivatorUtilities.CreateInstance(serviceProvider, ImplementationType, Array.Empty<object>());
            }

            var filter = _filterFactory.CreateInstance(serviceProvider);
            return filter;
        }
    }
}
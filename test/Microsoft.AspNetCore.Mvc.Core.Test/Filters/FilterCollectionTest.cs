// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Testing;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.Filters
{
    public class FilterCollectionTest
    {
        [Fact]
        public void Add_UsesTypeFilterAttribute()
        {
            // Arrange
            var collection = new FilterCollection();

            // Act
            var added = collection.Add(typeof(MyFilter));

            // Assert
            var typeFilter = Assert.IsType<TypeFilterAttribute>(added);
            Assert.Equal(typeof(MyFilter), typeFilter.ImplementationType);
            Assert.Same(typeFilter, Assert.Single(collection));
        }

        [Fact]
        public void GenericAdd_UsesTypeFilterAttribute()
        {
            // Arrange
            var collection = new FilterCollection();

            // Act
            var added = collection.Add<MyFilter>();

            // Assert
            var typeFilter = Assert.IsType<TypeFilterAttribute>(added);
            Assert.Equal(typeof(MyFilter), typeFilter.ImplementationType);
            Assert.Same(typeFilter, Assert.Single(collection));
        }

        [Fact]
        public void Add_WithOrder_SetsOrder()
        {
            // Arrange
            var collection = new FilterCollection();

            // Act
            var added = collection.Add(typeof(MyFilter), 17);

            // Assert
            Assert.Equal(17, Assert.IsAssignableFrom<IOrderedFilter>(added).Order);
        }

        [Fact]
        public void GenericAdd_WithOrder_SetsOrder()
        {
            // Arrange
            var collection = new FilterCollection();

            // Act
            var added = collection.Add<MyFilter>(17);

            // Assert
            Assert.Equal(17, Assert.IsAssignableFrom<IOrderedFilter>(added).Order);
        }

        [Fact]
        public void Add_ThrowsOnNonIFilter()
        {
            // Arrange
            var collection = new FilterCollection();

            var expectedMessage = $"The type '{typeof(NonFilter).FullName}' must derive from " + $"'{typeof(IFilterMetadata).FullName}'.";

            // Act & Assert
            ExceptionAssert.ThrowsArgument(
                () => collection.Add(typeof(NonFilter)),
                "filterType",
                expectedMessage);
        }

        [Fact]
        public void Add_UnwrapsFilterFactoriesWithEmptyConstructors()
        {
            // Arrange
            var collection = new FilterCollection();

            // Act
            var filter = collection.Add<TestFilterFactoryWithParameterlessCtor>();

            // Assert
            Assert.IsType<TestFilterFactoryWithParameterlessCtor>(filter);
        }

        [Fact]
        public void Add_DoesNotUnwrapFilterFactoriesWithParameteredConstructors()
        {
            // Arrange
            var collection = new FilterCollection();

            // Act
            var filter = collection.Add<TestFilterFactoryWithParameterCtor>();

            // Assert
            var typeFilter = Assert.IsType<TypeFilterAttribute>(filter);
            Assert.Equal(typeof(TestFilterFactoryWithParameterCtor), typeFilter.ImplementationType);
        }

        [Fact]
        public void Add_DoesNotUnwrapFilterFactoriesWithNonPublicEmptyConstructors()
        {
            // Arrange
            var collection = new FilterCollection();

            // Act
            var filter = collection.Add<TestFilterFactoryWithNonPublicCtor>();

            // Assert
            var typeFilter = Assert.IsType<TypeFilterAttribute>(filter);
            Assert.Equal(typeof(TestFilterFactoryWithNonPublicCtor), typeFilter.ImplementationType);
        }

        [Fact]
        public void Add_WithAuthorizeFilter()
        {
            // Arrange
            var collection = new FilterCollection();

            // Act
            var filter = collection.Add<AuthorizeFilter>();

            // Assert
            Assert.IsType<AuthorizeFilter>(filter);
        }

        [Fact]
        public void AddService_UsesServiceFilterAttribute()
        {
            // Arrange
            var collection = new FilterCollection();

            // Act
            var added = collection.AddService(typeof(MyFilter));

            // Assert
            var serviceFilter = Assert.IsType<ServiceFilterAttribute>(added);
            Assert.Equal(typeof(MyFilter), serviceFilter.ServiceType);
            Assert.Same(serviceFilter, Assert.Single(collection));
        }

        [Fact]
        public void GenericAddService_UsesServiceFilterAttribute()
        {
            // Arrange
            var collection = new FilterCollection();

            // Act
            var added = collection.AddService<MyFilter>();

            // Assert
            var serviceFilter = Assert.IsType<ServiceFilterAttribute>(added);
            Assert.Equal(typeof(MyFilter), serviceFilter.ServiceType);
            Assert.Same(serviceFilter, Assert.Single(collection));
        }

        [Fact]
        public void AddService_SetsOrder()
        {
            // Arrange
            var collection = new FilterCollection();

            // Act
            var added = collection.AddService(typeof(MyFilter), 17);

            // Assert
            Assert.Equal(17, Assert.IsAssignableFrom<IOrderedFilter>(added).Order);
        }

        [Fact]
        public void GenericAddService_SetsOrder()
        {
            // Arrange
            var collection = new FilterCollection();

            // Act
            var added = collection.AddService<MyFilter>(17);

            // Assert
            Assert.Equal(17, Assert.IsAssignableFrom<IOrderedFilter>(added).Order);
        }

        [Fact]
        public void AddService_ThrowsOnNonIFilter()
        {
            // Arrange
            var collection = new FilterCollection();

            var expectedMessage = $"The type '{typeof(NonFilter).FullName}' must derive from '{typeof(IFilterMetadata).FullName}'.";

            // Act & Assert
            ExceptionAssert.ThrowsArgument(
                () => { collection.AddService(typeof(NonFilter)); },
                "filterType",
                expectedMessage);
        }

        private class MyFilter : IFilterMetadata, IOrderedFilter
        {
            public int Order
            {
                get;
                set;
            }
        }

        private class NonFilter
        {
        }

        public class TestFilterFactoryWithParameterlessCtor : IFilterFactory, IOrderedFilter
        {
            public int Order { get; set; } = 1000;

            public bool IsReusable => true;

            public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) => null;
        }

        public class TestFilterFactoryWithNonPublicCtor : IFilterFactory, IOrderedFilter
        {
            private TestFilterFactoryWithNonPublicCtor() { }

            public int Order { get; set; } = 1000;

            public bool IsReusable => true;

            public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) => null;
        }

        public class TestFilterFactoryWithParameterCtor : IFilterFactory, IOrderedFilter
        {
            public TestFilterFactoryWithParameterCtor(object parameter) { }

            public int Order { get; set; } = 1000;

            public bool IsReusable => true;

            public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) => null;
        }
    }
}

﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Test
{
    public class MatchCollectionTests
    {
        [Fact]
        public static void EnumeratorTest1()
        {
            Regex regex = new Regex("e");
            MatchCollection collection = regex.Matches("dotnet");
            IEnumerator enumerator = collection.GetEnumerator();

            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.IsAssignableFrom<Match>(enumerator.Current);
            Assert.Equal(4, ((Match)enumerator.Current).Index);
            Assert.Equal("e", ((Match)enumerator.Current).Value);
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            enumerator.Reset();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.IsAssignableFrom<Match>(enumerator.Current);
            Assert.Equal(4, ((Match)enumerator.Current).Index);
            Assert.Equal("e", ((Match)enumerator.Current).Value);
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }

        [Fact]
        public static void EnumeratorTest2()
        {
            MatchCollection collection = CreateCollection();
            IEnumerator enumerator = collection.GetEnumerator();

            for (int i = 0; i < collection.Count; i++)
            {
                enumerator.MoveNext();

                Assert.Equal(enumerator.Current, collection[i]);
            }

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            enumerator.Reset();

            for (int i = 0; i < collection.Count; i++)
            {
                enumerator.MoveNext();

                Assert.Equal(enumerator.Current, collection[i]);
            }

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }

        [Fact]
        public static void Contains()
        {
            ICollection<Match> collection = CreateCollection();
            foreach (Match item in collection)
            {
                Assert.True(collection.Contains(item));
            }

            foreach (Match item in CreateCollection())
            {
                Assert.False(collection.Contains(item));
            }
        }

        [Fact]
        public static void ContainsNonGeneric()
        {
            IList collection = CreateCollection();
            foreach (object item in collection)
            {
                Assert.True(collection.Contains(item));
            }

            foreach (object item in CreateCollection())
            {
                Assert.False(collection.Contains(item));
            }

            Assert.False(collection.Contains(new object()));
        }

        [Fact]
        public static void IndexOf()
        {
            IList<Match> collection = CreateCollection();

            int i = 0;
            foreach (Match item in collection)
            {
                Assert.Equal(i, collection.IndexOf(item));
                i++;
            }

            foreach (Match item in CreateCollection())
            {
                Assert.Equal(-1, collection.IndexOf(item));
            }
        }

        [Fact]
        public static void IndexOfNonGeneric()
        {
            IList collection = CreateCollection();

            int i = 0;
            foreach (object item in collection)
            {
                Assert.Equal(i, collection.IndexOf(item));
                i++;
            }

            foreach (object item in CreateCollection())
            {
                Assert.Equal(-1, collection.IndexOf(item));
            }

            Assert.Equal(-1, collection.IndexOf(new object()));
        }

        [Fact]
        public static void Indexer()
        {
            MatchCollection collection = CreateCollection();
            Assert.Equal("t", collection[0].ToString());
            Assert.Equal("t", collection[1].ToString());
        }

        [Fact]
        public static void IndexerIListOfT()
        {
            IList<Match> collection = CreateCollection();
            Assert.Equal("t", collection[0].ToString());
            Assert.Equal("t", collection[1].ToString());
        }

        [Fact]
        public static void IndexerIListNonGeneric()
        {
            IList collection = CreateCollection();
            Assert.Equal("t", collection[0].ToString());
            Assert.Equal("t", collection[1].ToString());
        }

        [Fact]
        public static void CopyToExceptions()
        {
            MatchCollection collection = CreateCollection();
            Assert.Throws<ArgumentNullException>(() => collection.CopyTo(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(new Match[1], -1));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new Match[1], 0));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new Match[1], 1));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new Match[1], 2));
        }

        [Fact]
        public static void CopyToNonGenericExceptions()
        {
            ICollection collection = CreateCollection();
            Assert.Throws<ArgumentNullException>(() => collection.CopyTo(null, 0));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new Match[10, 10], 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(new Match[1], -1));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new Match[1], 0));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new Match[1], 1));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new Match[1], 2));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new int[collection.Count], 0));
        }

        [Fact]
        public static void CopyTo()
        {
            string[] expected = new[] { "t", "t" };
            MatchCollection collection = CreateCollection();

            Match[] array = new Match[collection.Count];
            collection.CopyTo(array, 0);

            Assert.Equal(expected, array.Select(c => c.ToString()));
        }

        [Fact]
        public static void CopyToNonGeneric()
        {
            string[] expected = new[] { "t", "t" };
            ICollection collection = CreateCollection();

            Capture[] array = new Capture[collection.Count];
            collection.CopyTo(array, 0);

            Assert.Equal(expected, array.Select(c => c.ToString()));
        }

        [Fact]
        public static void SyncRoot()
        {
            ICollection collection = CreateCollection();
            Assert.NotNull(collection.SyncRoot);
            Assert.Same(collection.SyncRoot, collection.SyncRoot);
        }

        [Fact]
        public static void IsNotSynchronized()
        {
            ICollection collection = CreateCollection();
            Assert.False(collection.IsSynchronized);
        }

        [Fact]
        public void IsReadOnly()
        {
            IList<Match> list = CreateCollection();
            Assert.True(list.IsReadOnly);
            Assert.Throws<NotSupportedException>(() => list.Add(default(Match)));
            Assert.Throws<NotSupportedException>(() => list.Clear());
            Assert.Throws<NotSupportedException>(() => list.Insert(0, default(Match)));
            Assert.Throws<NotSupportedException>(() => list.Remove(default(Match)));
            Assert.Throws<NotSupportedException>(() => list.RemoveAt(0));
            Assert.Throws<NotSupportedException>(() => list[0] = default(Match));
        }

        [Fact]
        public static void IsReadOnlyNonGeneric()
        {
            IList list = CreateCollection();
            Assert.True(list.IsReadOnly);
            Assert.True(list.IsFixedSize);
            Assert.Throws<NotSupportedException>(() => list.Add(default(Match)));
            Assert.Throws<NotSupportedException>(() => list.Clear());
            Assert.Throws<NotSupportedException>(() => list.Insert(0, default(Match)));
            Assert.Throws<NotSupportedException>(() => list.Remove(default(Match)));
            Assert.Throws<NotSupportedException>(() => list.RemoveAt(0));
            Assert.Throws<NotSupportedException>(() => list[0] = default(Match));
        }

        [Fact]
        public static void DebuggerAttributeTests()
        {
            DebuggerAttributes.ValidateDebuggerDisplayReferences(CreateCollection());
            DebuggerAttributes.ValidateDebuggerTypeProxyProperties(CreateCollection());
        }

        private static MatchCollection CreateCollection()
        {
            return new Regex("t").Matches("dotnet");
        }
    }
}

﻿// Copyright (c) 2012-2020 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

using FellowOakDicom.IO;
using System;
using System.IO;
using Xunit;

namespace FellowOakDicom.Tests.IO
{

    [Collection("General")]
    public class TemporaryFileTest
    {
        #region Fields

        private readonly object locker = new object();

        #endregion

        #region Unit tests

        [Fact]
        public void StoragePath_Setter_DirectoryCreatedIfNonExisting()
        {
            lock (this.locker)
            {
                var path = TestData.Resolve("Temporary Path 1");
                if (Directory.Exists(path)) Directory.Delete(path, true);

                TemporaryFile.StoragePath = path;
                Assert.True(Directory.Exists(path));
            }
        }

        [Fact]
        public void StoragePath_Setter_NullShouldNotThrow()
        {
            TemporaryFile.StoragePath = null;
        }

        [Fact]
        public void StoragePath_Getter_DefaultEqualToUserTemp()
        {
            lock (this.locker)
            {
                var expected = Path.GetTempPath();

                TemporaryFile.StoragePath = null;
                var actual = TemporaryFile.StoragePath;

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void Create_StoragePathNull_LocatedInUserTemp()
        {
            lock (this.locker)
            {
                TemporaryFile.StoragePath = null;
                var temp = TemporaryFile.Create().Name;

                var expected = Path.GetTempPath().TrimEnd('\\').TrimEnd('/');
                var actual = Path.GetDirectoryName(temp);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void Create_StoragePathNonNull_LocatedInSpecDirectory()
        {
            lock (this.locker)
            {
                var expected = TestData.Resolve("Temporary Path 2");
                TemporaryFile.StoragePath = expected;

                var temp = TemporaryFile.Create().Name;

                var actual = Path.GetDirectoryName(temp);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void Create_StoragePathNonNull_FileAttributesContainTempFlag()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                lock (this.locker)
                {
                    TemporaryFile.StoragePath = TestData.Resolve("Temporary Path 3");
                    var path = TemporaryFile.Create().Name;
                    Assert.True((File.GetAttributes(path) & FileAttributes.Temporary) == FileAttributes.Temporary);
                }
            }
        }

        [Fact]
        public void Create_StoragePathNull_FileAttributesContainTempFlag()
        {
            lock (this.locker)
            {
                TemporaryFile.StoragePath = null;
                var path = TemporaryFile.Create().Name;
                Assert.True((File.GetAttributes(path) & FileAttributes.Temporary) == FileAttributes.Temporary);
            }
        }

        #endregion
    }
}

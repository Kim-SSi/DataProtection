// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Cryptography.SafeHandles;

namespace Microsoft.AspNetCore.Cryptography.Cng
{
    internal static class OSVersionUtil
    {
        private static readonly OSVersion _osVersion = GetOSVersion();

        private static OSVersion GetOSVersion()
        {
            const string BCRYPT_LIB = "bcrypt.dll";
            SafeLibraryHandle bcryptLibHandle = null;
            try
            {
                bcryptLibHandle = SafeLibraryHandle.Open(BCRYPT_LIB);
            }
            catch
            {
                // we'll handle the exceptional case later
            }

            if (bcryptLibHandle != null)
            {
                using (bcryptLibHandle)
                {
                    if (bcryptLibHandle.DoesProcExist("BCryptKeyDerivation"))
                    {
                        // We're running on Win8+.
                        return OSVersion.Win8OrLater;
                    }
                    if (bcryptLibHandle.DoesProcExist("BCryptDeriveKeyPBKDF2"))
                    {
                        // We're running on Win7+.
                        return OSVersion.Win7OrLater;
                    }
					// We're running on Vista+.
					return OSVersion.WinVistaOrLater;
                }
            }
            else
            {
                // Not running on Win7+.
                return OSVersion.NotWindows;
            }
        }

		public static bool IsWindows()
        {
            return (_osVersion >= OSVersion.WinVistaOrLater);
        }
		
        public static bool IsWindows7OrLater()
        {
            return (_osVersion >= OSVersion.Win7OrLater);
        }

        public static bool IsWindows8OrLater()
        {
            return (_osVersion >= OSVersion.Win8OrLater);
        }

        private enum OSVersion
        {
            NotWindows = 0,
            WinVistaOrLater = 1,
            Win7OrLater = 2,
            Win8OrLater = 3
        }
    }
}

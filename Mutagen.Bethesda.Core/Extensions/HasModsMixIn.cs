using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class HasModsMixIn
    {
        /// <summary>
        /// Checks whether a given mod is in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="modKey">ModKey to look for</param>
        /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
        /// <returns>True if ModKey is in the listings, with the desired enabled state</returns>
        public static bool HasMod(this IEnumerable<LoadOrderListing> listings, ModKey modKey, bool? enabled = null)
        {
            foreach (var listing in listings)
            {
                if (listing.ModKey == modKey
                    && (enabled == null || listing.Enabled == enabled))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Asserts a given mod is in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="modKey">ModKey to look for</param>
        /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
        /// <param name="message">Message to attach to exception if mod doesn't exist</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMod(this IEnumerable<LoadOrderListing> listings, ModKey modKey, bool? enabled = null, string? message = null)
        {
            if (!HasMod(listings, modKey, enabled))
            {
                throw new MissingModException(modKey, message: message);
            }
        }

        /// <summary>
        /// Checks whether all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <returns>True if all ModKeys are present in the listings</returns>
        public static bool HasMods(this IEnumerable<LoadOrderListing> listings, params ModKey[] modKeys)
        {
            return HasMods(listings.Select(x => x.ModKey), modKeys);
        }

        /// <summary>
        /// Asserts all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMods(this IEnumerable<LoadOrderListing> listings, params ModKey[] modKeys)
        {
            AssertHasMods(listings, message: null, modKeys);
        }

        /// <summary>
        /// Asserts all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="message">Message to attach to exception if mod doesn't exist</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMods(this IEnumerable<LoadOrderListing> listings, string? message, params ModKey[] modKeys)
        {
            AssertHasMods(listings.Select(x => x.ModKey), message, modKeys);
        }

        /// <summary>
        /// Checks whether all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <returns>True if all ModKeys are present in the listings</returns>
        public static bool HasMods(this IEnumerable<LoadOrderListing> listings, IEnumerable<ModKey> modKeys)
        {
            return HasMods(listings, modKeys.ToArray());
        }

        /// <summary>
        /// Asserts all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <param name="message">Message to attach to exception if mod doesn't exist</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMods(this IEnumerable<LoadOrderListing> listings, IEnumerable<ModKey> modKeys, string? message = null)
        {
            AssertHasMods(listings, message, modKeys.ToArray());
        }

        /// <summary>
        /// Checks whether all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
        /// <param name="modKeys">ModKey to look for</param>
        /// <returns>True if all ModKeys are present in the listings, with the desired enabled state</returns>
        public static bool HasMods(this IEnumerable<LoadOrderListing> listings, bool enabled, params ModKey[] modKeys)
        {
            if (modKeys.Length == 0) return true;
            if (modKeys.Length == 1) return HasMod(listings, modKeys[0], enabled);
            var set = modKeys.ToHashSet();
            foreach (var listing in listings)
            {
                if (listing.Enabled == enabled
                    && set.Remove(listing.ModKey)
                    && set.Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Asserts all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
        /// <param name="modKeys">ModKey to look for</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMods(this IEnumerable<LoadOrderListing> listings, bool enabled, params ModKey[] modKeys)
        {
            AssertHasMods(listings, enabled, message: null, modKeys: modKeys);
        }

        /// <summary>
        /// Assertsall of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
        /// <param name="message">Message to attach to exception if mod doesn't exist</param>
        /// <param name="modKeys">ModKey to look for</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMods(this IEnumerable<LoadOrderListing> listings, bool enabled, string? message, params ModKey[] modKeys)
        {
            if (modKeys.Length == 0) return;
            if (modKeys.Length == 1) AssertHasMod(listings, modKeys[0], enabled);
            var set = modKeys.ToHashSet();
            foreach (var listing in listings)
            {
                if (listing.Enabled == enabled
                    && set.Remove(listing.ModKey)
                    && set.Count == 0)
                {
                    return;
                }
            }
            if (set.Count > 0)
            {
                throw new MissingModException(set, message: message);
            }
        }

        /// <summary>
        /// Checks whether all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
        /// <param name="modKeys">ModKey to look for</param>
        /// <returns>True if all ModKeys are present in the listings, with the desired enabled state</returns>
        public static bool HasMods(this IEnumerable<LoadOrderListing> listings, bool enabled, IEnumerable<ModKey> modKeys)
        {
            return HasMods(listings, enabled, modKeys.ToArray());
        }

        /// <summary>
        /// Asserts all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
        /// <param name="modKeys">ModKey to look for</param>
        /// <param name="message">Message to attach to exception if mod doesn't exist</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMods(this IEnumerable<LoadOrderListing> listings, bool enabled, IEnumerable<ModKey> modKeys, string? message = null)
        {
            AssertHasMods(listings, enabled, message: message, modKeys: modKeys.ToArray());
        }

        /// <summary>
        /// Checks whether a given mod is in the collection of keys.
        /// </summary>
        /// <param name="keys">Listings to look through</param>
        /// <param name="modKey">ModKey to look for</param>
        /// <returns>True if ModKey is in the listings, with the desired enabled state</returns>
        public static bool HasMod(this IEnumerable<ModKey> keys, ModKey modKey)
        {
            return keys.Contains(modKey);
        }

        /// <summary>
        /// Asserts a given mod is in the collection of keys.
        /// </summary>
        /// <param name="keys">Listings to look through</param>
        /// <param name="modKey">ModKey to look for</param>
        /// <param name="message">Message to attach to exception if mod doesn't exist</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMod(this IEnumerable<ModKey> keys, ModKey modKey, string? message = null)
        {
            if (!HasMod(keys, modKey))
            {
                throw new MissingModException(keys, message: message);
            }
        }

        /// <summary>
        /// Checks whether all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="keys">Listings to look through</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <returns>True if all ModKeys are present in the listings</returns>
        public static bool HasMods(this IEnumerable<ModKey> keys, params ModKey[] modKeys)
        {
            if (modKeys.Length == 0) return true;
            if (modKeys.Length == 1) return HasMod(keys, modKeys[0]);
            var set = modKeys.ToHashSet();
            foreach (var listing in keys)
            {
                if (set.Remove(listing)
                    && set.Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Asserts all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="keys">Listings to look through</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMods(this IEnumerable<ModKey> keys, params ModKey[] modKeys)
        {
            AssertHasMods(keys, message: null, modKeys);
        }

        /// <summary>
        /// Asserts all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="keys">Listings to look through</param>
        /// <param name="message">Message to attach to exception if mod doesn't exist</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMods(this IEnumerable<ModKey> keys, string? message, params ModKey[] modKeys)
        {
            if (modKeys.Length == 0) return;
            if (modKeys.Length == 1) AssertHasMod(keys, modKeys[0]);
            var set = modKeys.ToHashSet();
            foreach (var listing in keys)
            {
                if (set.Remove(listing)
                    && set.Count == 0)
                {
                    return;
                }
            }
            if (set.Count > 0)
            {
                throw new MissingModException(set, message: message);
            }
        }

        /// <summary>
        /// Checks whether all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="keys">Listings to look through</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <returns>True if all ModKeys are present in the listings</returns>
        public static bool HasMods(this IEnumerable<ModKey> keys, IEnumerable<ModKey> modKeys)
        {
            return HasMods(keys, modKeys.ToArray());
        }

        /// <summary>
        /// Asserts all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="keys">Listings to look through</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <param name="message">Message to attach to exception if mod doesn't exist</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMods(this IEnumerable<ModKey> keys, IEnumerable<ModKey> modKeys, string? message = null)
        {
            AssertHasMods(keys, message: message, modKeys: modKeys.ToArray());
        }

        /// <summary>
        /// Checks whether a given mod is in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="modKey">ModKey to look for</param>
        /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
        /// <returns>True if ModKey is in the listings, with the desired enabled state</returns>
        public static bool HasMod<TMod>(this IEnumerable<IModListing<TMod>> listings, ModKey modKey, bool? enabled = null)
            where TMod : class, IModGetter
        {
            return listings.Select(m => new LoadOrderListing(m.ModKey, m.Enabled)).HasMod(modKey, enabled);
        }

        /// <summary>
        /// Asserts a given mod is in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="modKey">ModKey to look for</param>
        /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
        /// <param name="message">Message to attach to exception if mod doesn't exist</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMod<TMod>(this IEnumerable<IModListing<TMod>> listings, ModKey modKey, bool? enabled = null, string? message = null)
            where TMod : class, IModGetter
        {
            AssertHasMod(listings.Select(m => new LoadOrderListing(m.ModKey, m.Enabled)), enabled: enabled, modKey: modKey);
        }

        /// <summary>
        /// Checks whether all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <returns>True if all ModKeys are present in the listings</returns>
        public static bool HasMods<TMod>(this IEnumerable<IModListing<TMod>> listings, params ModKey[] modKeys)
            where TMod : class, IModGetter
        {
            return listings.Select(m => new LoadOrderListing(m.ModKey, m.Enabled)).HasMods(modKeys);
        }

        /// <summary>
        /// Asserts all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <returns>True if all ModKeys are present in the listings</returns>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMods<TMod>(this IEnumerable<IModListing<TMod>> listings, params ModKey[] modKeys)
            where TMod : class, IModGetter
        {
            AssertHasMods(listings.Select(m => m.ModKey), modKeys: modKeys);
        }

        /// <summary>
        /// Asserts all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <returns>True if all ModKeys are present in the listings</returns>
        /// <param name="message">Message to attach to exception if mod doesn't exist</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMods<TMod>(this IEnumerable<IModListing<TMod>> listings, string message, params ModKey[] modKeys)
            where TMod : class, IModGetter
        {
            AssertHasMods(listings.Select(m => m.ModKey), modKeys: modKeys, message: message);
        }

        /// <summary>
        /// Checks whether all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <returns>True if all ModKeys are present in the listings</returns>
        public static bool HasMods<TMod>(this IEnumerable<IModListing<TMod>> listings, IEnumerable<ModKey> modKeys)
            where TMod : class, IModGetter
        {
            return listings.Select(m => m.ModKey).HasMods(modKeys);
        }

        /// <summary>
        /// Asserts all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <param name="message">Message to attach to exception if mod doesn't exist</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMods<TMod>(this IEnumerable<IModListing<TMod>> listings, IEnumerable<ModKey> modKeys, string? message = null)
            where TMod : class, IModGetter
        {
            AssertHasMods(listings.Select(m => m.ModKey), message: message, modKeys: modKeys.ToArray());
        }

        /// <summary>
        /// Checks whether all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
        /// <param name="present">Whether the ModKey should be present on disk</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <returns>True if all ModKeys are present in the listings</returns>
        public static bool HasMods<TMod>(this IEnumerable<IModListing<TMod>> listings, bool? enabled, bool? present, params ModKey[] modKeys)
            where TMod : class, IModGetter
        {
            if (modKeys.Length == 0) return true;
            if (modKeys.Length == 1) return HasMod(listings, modKeys[0], enabled);
            var set = modKeys.ToHashSet();
            foreach (var listing in listings)
            {
                if (enabled != null && listing.Enabled != enabled) continue;
                if (present != null && (listing.Mod != null) != present) continue;
                if (set.Remove(listing.ModKey)
                    && set.Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Asserts all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
        /// <param name="present">Whether the ModKey should be present on disk</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMods<TMod>(this IEnumerable<IModListing<TMod>> listings, bool? enabled, bool? present, params ModKey[] modKeys)
            where TMod : class, IModGetter
        {
            AssertHasMods(listings, enabled: enabled, present: present, message: null, modKeys: modKeys);
        }

        /// <summary>
        /// Asserts all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
        /// <param name="present">Whether the ModKey should be present on disk</param>
        /// <param name="message">Message to attach to exception if mod doesn't exist</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMods<TMod>(this IEnumerable<IModListing<TMod>> listings, bool? enabled, bool? present, string? message, params ModKey[] modKeys)
            where TMod : class, IModGetter
        {
            if (modKeys.Length == 0) return;
            if (modKeys.Length == 1) AssertHasMod(listings, modKeys[0], enabled);
            var set = modKeys.ToHashSet();
            foreach (var listing in listings)
            {
                if (enabled != null && listing.Enabled != enabled) continue;
                if (present != null && (listing.Mod != null) != present) continue;
                if (set.Remove(listing.ModKey)
                    && set.Count == 0)
                {
                    return;
                }
            }

            if (set.Count > 0)
            {
                throw new MissingModException(set, message: message);
            }
        }

        /// <summary>
        /// Checks whether all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
        /// <param name="present">Whether the ModKey should be present on disk</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <returns>True if all ModKeys are present in the listings</returns>
        public static bool HasMods<TMod>(this IEnumerable<IModListing<TMod>> listings, bool? enabled, bool? present, IEnumerable<ModKey> modKeys)
            where TMod : class, IModGetter
        {
            return HasMods(listings, enabled, present, modKeys.ToArray());
        }

        /// <summary>
        /// Asserts all of a given set of ModKeys are in the collection of listings.
        /// </summary>
        /// <param name="listings">Listings to look through</param>
        /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
        /// <param name="present">Whether the ModKey should be present on disk</param>
        /// <param name="message">Message to attach to exception if mod doesn't exist</param>
        /// <param name="modKeys">ModKeys to look for</param>
        /// <exception cref="MissingModException">
        /// Thrown if given mod is not on the collection of listings
        /// </exception>
        public static void AssertHasMods<TMod>(this IEnumerable<IModListing<TMod>> listings, bool? enabled, bool? present, IEnumerable<ModKey> modKeys, string? message = null)
            where TMod : class, IModGetter
        {
            AssertHasMods(listings, enabled: enabled, present: present, message: message, modKeys: modKeys.ToArray());
        }
    }
}

Shuffle Table Generator
=======================

The `LicenseKey` class converts a GUID (or any 128 bits of data) into a license key similar
to the ones used by Microsoft products (5 groups of 5 alphanumeric characters).

For additional obfuscation, bits can be mapped in a random order. That will offer some
protection when you have, for example, a running serial number. With shuffling, each
increment will result in likely several alphanumeric characters changing in seemingly
unpredictable ways (base-2 to base-36 conversion pretty much guarantees that any small
change affects many following alphanumeric "digits").

    /// <summary>Generates a new random shuffle table</summary>
    /// <param name="iterationCount">
    ///   Number of iterations in which to randomize the shuffle table
    /// </param>
    /// <returns>The new random shuffle table</returns>
    public static byte[] generateShuffleTable(int iterationCount) {
      byte[] shuffleTable = new byte[128];
      for(int index = 0; index < 128; ++index)
        shuffleTable[index] = index;

      Random rng = new Random();

      for(int iteration = 0; iteration < iterationCount; ++iteration) {
        int firstIndex = rng.Next() % 128;
        int secondIndex = rng.Next() % 128;

        byte temp = shuffleTable[firstIndex];
        shuffleTable[firstIndex] = shuffleTable[secondIndex];
        shuffleTable[secondIndex] = temp;
      }

      return shuffleTable;      
    }

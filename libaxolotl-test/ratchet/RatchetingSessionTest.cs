﻿using libaxolotl;
using libaxolotl.ecc;
using libaxolotl.ratchet;
using libaxolotl.state;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Strilanc.Value;

namespace libaxolotl_test
{
    [TestClass]
    public class RatchetingSessionTest
    {
        [TestMethod, TestCategory("libaxolotl.ratchet")]
        public void testRatchetingSessionAsBob()
        {
            byte[] bobPublic =
            {
                0x05, 0x2c, 0xb4, 0x97,
                0x76, 0xb8, 0x77, 0x02,
                0x05, 0x74, 0x5a, 0x3a,
                0x6e, 0x24, 0xf5, 0x79,
                0xcd, 0xb4, 0xba, 0x7a,
                0x89, 0x04, 0x10, 0x05,
                0x92, 0x8e, 0xbb, 0xad,
                0xc9, 0xc0, 0x5a, 0xd4,
                0x58
            };

            byte[] bobPrivate =
            {
                0xa1, 0xca, 0xb4, 0x8f,
                0x7c, 0x89, 0x3f, 0xaf,
                0xa9, 0x88, 0x0a, 0x28,
                0xc3, 0xb4, 0x99, 0x9d,
                0x28, 0xd6, 0x32, 0x95,
                0x62, 0xd2, 0x7a, 0x4e,
                0xa4, 0xe2, 0x2e, 0x9f,
                0xf1, 0xbd, 0xd6, 0x5a
            };

            byte[] bobIdentityPublic =
            {
                0x05, 0xf1, 0xf4, 0x38,
                0x74, 0xf6, 0x96, 0x69,
                0x56, 0xc2, 0xdd, 0x47,
                0x3f, 0x8f, 0xa1, 0x5a,
                0xde, 0xb7, 0x1d, 0x1c,
                0xb9, 0x91, 0xb2, 0x34,
                0x16, 0x92, 0x32, 0x4c,
                0xef, 0xb1, 0xc5, 0xe6,
                0x26
            };

            byte[] bobIdentityPrivate =
            {
                0x48, 0x75, 0xcc, 0x69,
                0xdd, 0xf8, 0xea, 0x07,
                0x19, 0xec, 0x94, 0x7d,
                0x61, 0x08, 0x11, 0x35,
                0x86, 0x8d, 0x5f, 0xd8,
                0x01, 0xf0, 0x2c, 0x02,
                0x25, 0xe5, 0x16, 0xdf,
                0x21, 0x56, 0x60, 0x5e
            };

            byte[] aliceBasePublic =
            {
                0x05, 0x47, 0x2d, 0x1f,
                0xb1, 0xa9, 0x86, 0x2c,
                0x3a, 0xf6, 0xbe, 0xac,
                0xa8, 0x92, 0x02, 0x77,
                0xe2, 0xb2, 0x6f, 0x4a,
                0x79, 0x21, 0x3e, 0xc7,
                0xc9, 0x06, 0xae, 0xb3,
                0x5e, 0x03, 0xcf, 0x89,
                0x50
            };

            byte[] aliceEphemeralPublic =
            {
                0x05, 0x6c, 0x3e, 0x0d,
                0x1f, 0x52, 0x02, 0x83,
                0xef, 0xcc, 0x55, 0xfc,
                0xa5, 0xe6, 0x70, 0x75,
                0xb9, 0x04, 0x00, 0x7f,
                0x18, 0x81, 0xd1, 0x51,
                0xaf, 0x76, 0xdf, 0x18,
                0xc5, 0x1d, 0x29, 0xd3,
                0x4b
            };

            byte[] aliceIdentityPublic =
            {
                0x05, 0xb4, 0xa8, 0x45,
                0x56, 0x60, 0xad, 0xa6,
                0x5b, 0x40, 0x10, 0x07,
                0xf6, 0x15, 0xe6, 0x54,
                0x04, 0x17, 0x46, 0x43,
                0x2e, 0x33, 0x39, 0xc6,
                0x87, 0x51, 0x49, 0xbc,
                0xee, 0xfc, 0xb4, 0x2b,
                0x4a
            };

            byte[] senderChain =
            {
                0xd2, 0x2f, 0xd5, 0x6d, 0x3f,
                0xec, 0x81, 0x9c, 0xf4, 0xc3,
                0xd5, 0x0c, 0x56, 0xed, 0xfb,
                0x1c, 0x28, 0x0a, 0x1b, 0x31,
                0x96, 0x45, 0x37, 0xf1, 0xd1,
                0x61, 0xe1, 0xc9, 0x31, 0x48,
                0xe3, 0x6b
            };

            IdentityKey bobIdentityKeyPublic = new IdentityKey(bobIdentityPublic, 0);
            ECPrivateKey bobIdentityKeyPrivate = Curve.decodePrivatePoint(bobIdentityPrivate);
            IdentityKeyPair bobIdentityKey = new IdentityKeyPair(bobIdentityKeyPublic, bobIdentityKeyPrivate);
            ECPublicKey bobEphemeralPublicKey = Curve.decodePoint(bobPublic, 0);
            ECPrivateKey bobEphemeralPrivateKey = Curve.decodePrivatePoint(bobPrivate);
            ECKeyPair bobEphemeralKey = new ECKeyPair(bobEphemeralPublicKey, bobEphemeralPrivateKey);
            ECKeyPair bobBaseKey = bobEphemeralKey;

            ECPublicKey aliceBasePublicKey = Curve.decodePoint(aliceBasePublic, 0);
            ECPublicKey aliceEphemeralPublicKey = Curve.decodePoint(aliceEphemeralPublic, 0);
            IdentityKey aliceIdentityPublicKey = new IdentityKey(aliceIdentityPublic, 0);

            BobAxolotlParameters parameters = BobAxolotlParameters.newBuilder()
                .setOurIdentityKey(bobIdentityKey)
                .setOurSignedPreKey(bobBaseKey)
                .setOurRatchetKey(bobEphemeralKey)
                .setOurOneTimePreKey(May<ECKeyPair>.NoValue)
                .setTheirIdentityKey(aliceIdentityPublicKey)
                .setTheirBaseKey(aliceBasePublicKey)
                .create();

            SessionState session = new SessionState();

            RatchetingSession.initializeSession(session, 2, parameters);

            Assert.AreEqual(session.getLocalIdentityKey(), bobIdentityKey.getPublicKey());
            Assert.AreEqual(session.getRemoteIdentityKey(), aliceIdentityPublicKey);
            CollectionAssert.AreEqual(session.getSenderChainKey().getKey(), senderChain);
        }

        [TestMethod, TestCategory("libaxolotl.ratchet")]
        public void testRatchetingSessionAsAlice()
        {
            byte[] bobPublic =
            {
                0x05, 0x2c, 0xb4, 0x97, 0x76,
                0xb8, 0x77, 0x02, 0x05, 0x74,
                0x5a, 0x3a, 0x6e, 0x24, 0xf5,
                0x79, 0xcd, 0xb4, 0xba, 0x7a,
                0x89, 0x04, 0x10, 0x05, 0x92,
                0x8e, 0xbb, 0xad, 0xc9, 0xc0,
                0x5a, 0xd4, 0x58
            };

            byte[] bobIdentityPublic =
            {
                0x05, 0xf1, 0xf4, 0x38, 0x74,
                0xf6, 0x96, 0x69, 0x56, 0xc2,
                0xdd, 0x47, 0x3f, 0x8f, 0xa1,
                0x5a, 0xde, 0xb7, 0x1d, 0x1c,
                0xb9, 0x91, 0xb2, 0x34, 0x16,
                0x92, 0x32, 0x4c, 0xef, 0xb1,
                0xc5, 0xe6, 0x26
            };

            byte[] aliceBasePublic =
            {
                0x05, 0x47, 0x2d, 0x1f, 0xb1,
                0xa9, 0x86, 0x2c, 0x3a, 0xf6,
                0xbe, 0xac, 0xa8, 0x92, 0x02,
                0x77, 0xe2, 0xb2, 0x6f, 0x4a,
                0x79, 0x21, 0x3e, 0xc7, 0xc9,
                0x06, 0xae, 0xb3, 0x5e, 0x03,
                0xcf, 0x89, 0x50
            };

            byte[] aliceBasePrivate =
            {
                0x11, 0xae, 0x7c, 0x64, 0xd1,
                0xe6, 0x1c, 0xd5, 0x96, 0xb7,
                0x6a, 0x0d, 0xb5, 0x01, 0x26,
                0x73, 0x39, 0x1c, 0xae, 0x66,
                0xed, 0xbf, 0xcf, 0x07, 0x3b,
                0x4d, 0xa8, 0x05, 0x16, 0xa4,
                0x74, 0x49
            };

            byte[] aliceEphemeralPublic =
            {
                0x05, 0x6c, 0x3e, 0x0d, 0x1f,
                0x52, 0x02, 0x83, 0xef, 0xcc,
                0x55, 0xfc, 0xa5, 0xe6, 0x70,
                0x75, 0xb9, 0x04, 0x00, 0x7f,
                0x18, 0x81, 0xd1, 0x51, 0xaf,
                0x76, 0xdf, 0x18, 0xc5, 0x1d,
                0x29, 0xd3, 0x4b
            };

            byte[] aliceEphemeralPrivate =
            {
                0xd1, 0xba, 0x38, 0xce, 0xa9,
                0x17, 0x43, 0xd3, 0x39, 0x39,
                0xc3, 0x3c, 0x84, 0x98, 0x65,
                0x09, 0x28, 0x01, 0x61, 0xb8,
                0xb6, 0x0f, 0xc7, 0x87, 0x0c,
                0x59, 0x9c, 0x1d, 0x46, 0x20,
                0x12, 0x48
            };

            byte[] aliceIdentityPublic =
            {
                0x05, 0xb4, 0xa8, 0x45, 0x56,
                0x60, 0xad, 0xa6, 0x5b, 0x40,
                0x10, 0x07, 0xf6, 0x15, 0xe6,
                0x54, 0x04, 0x17, 0x46, 0x43,
                0x2e, 0x33, 0x39, 0xc6, 0x87,
                0x51, 0x49, 0xbc, 0xee, 0xfc,
                0xb4, 0x2b, 0x4a
            };

            byte[] aliceIdentityPrivate =
            {
                0x90, 0x40, 0xf0, 0xd4, 0xe0,
                0x9c, 0xf3, 0x8f, 0x6d, 0xc7,
                0xc1, 0x37, 0x79, 0xc9, 0x08,
                0xc0, 0x15, 0xa1, 0xda, 0x4f,
                0xa7, 0x87, 0x37, 0xa0, 0x80,
                0xeb, 0x0a, 0x6f, 0x4f, 0x5f,
                0x8f, 0x58
            };

            byte[] receiverChain =
            {
                0xd2, 0x2f, 0xd5, 0x6d, 0x3f,
                0xec, 0x81, 0x9c, 0xf4, 0xc3,
                0xd5, 0x0c, 0x56, 0xed, 0xfb,
                0x1c, 0x28, 0x0a, 0x1b, 0x31,
                0x96, 0x45, 0x37, 0xf1, 0xd1,
                0x61, 0xe1, 0xc9, 0x31, 0x48,
                0xe3, 0x6b
            };

            IdentityKey bobIdentityKey = new IdentityKey(bobIdentityPublic, 0);
            ECPublicKey bobEphemeralPublicKey = Curve.decodePoint(bobPublic, 0);
            ECPublicKey bobBasePublicKey = bobEphemeralPublicKey;
            ECPublicKey aliceBasePublicKey = Curve.decodePoint(aliceBasePublic, 0);
            ECPrivateKey aliceBasePrivateKey = Curve.decodePrivatePoint(aliceBasePrivate);
            ECKeyPair aliceBaseKey = new ECKeyPair(aliceBasePublicKey, aliceBasePrivateKey);
            ECPublicKey aliceEphemeralPublicKey = Curve.decodePoint(aliceEphemeralPublic, 0);
            ECPrivateKey aliceEphemeralPrivateKey = Curve.decodePrivatePoint(aliceEphemeralPrivate);
            ECKeyPair aliceEphemeralKey = new ECKeyPair(aliceEphemeralPublicKey, aliceEphemeralPrivateKey);
            IdentityKey aliceIdentityPublicKey = new IdentityKey(aliceIdentityPublic, 0);
            ECPrivateKey aliceIdentityPrivateKey = Curve.decodePrivatePoint(aliceIdentityPrivate);
            IdentityKeyPair aliceIdentityKey = new IdentityKeyPair(aliceIdentityPublicKey, aliceIdentityPrivateKey);

            SessionState session = new SessionState();

            AliceAxolotlParameters parameters = AliceAxolotlParameters.newBuilder()
                .setOurBaseKey(aliceBaseKey)
                .setOurIdentityKey(aliceIdentityKey)
                .setTheirIdentityKey(bobIdentityKey)
                .setTheirSignedPreKey(bobBasePublicKey)
                .setTheirRatchetKey(bobEphemeralPublicKey)
                .setTheirOneTimePreKey(May<ECPublicKey>.NoValue)
                .create();

            RatchetingSession.initializeSession(session, 2, parameters);

            Assert.AreEqual(session.getLocalIdentityKey(), aliceIdentityKey.getPublicKey());
            Assert.AreEqual(session.getRemoteIdentityKey(), bobIdentityKey);
            CollectionAssert.AreEqual(session.getReceiverChainKey(bobEphemeralPublicKey).getKey(), receiverChain);
        }
    }
}


namespace MMALSharp.Config
{
    public enum BayerOrder
    {
		//Carefully ordered so that an hflip is ^1,
		//and a vflip is ^2.
		BAYER_ORDER_BGGR,
		BAYER_ORDER_GBRG,
		BAYER_ORDER_GRBG,
		BAYER_ORDER_RGGB
	}
}

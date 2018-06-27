using System;
namespace Xamarin.SNS.Login.Sample.Models.Providers
{
    public class OAuth2ProviderFactory
    {
        public static OAuth2Base CreateProvider(SNSProvider provider)
        {
            OAuth2Base oAuth2 = null;

            switch (provider)
            {
                case SNSProvider.Kakao:
                    oAuth2 = KakaoOAuth2.Instance;
                    break;
                case SNSProvider.Line:
                    oAuth2 = LineOAuth2.Instance;
                    break;
                case SNSProvider.Facebook:
                    oAuth2 = FacebookOAuth2.Instance;
                    break;
            }

            return oAuth2;
        }
    }
}

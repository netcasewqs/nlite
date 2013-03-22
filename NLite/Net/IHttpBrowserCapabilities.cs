using System;
namespace NLite.Net
{
    public interface IHttpBrowserCapabilities 
    {
        bool ActiveXControls { get; }
        System.Collections.IDictionary Adapters { get; }
        void AddBrowser(string browserName);
        bool AOL { get; }
        bool BackgroundSounds { get; }
        bool Beta { get; }
        string Browser { get; }
        System.Collections.ArrayList Browsers { get; }
        bool CanCombineFormsInDeck { get; }
        bool CanInitiateVoiceCall { get; }
        bool CanRenderAfterInputOrSelectElement { get; }
        bool CanRenderEmptySelects { get; }
        bool CanRenderInputAndSelectElementsTogether { get; }
        bool CanRenderMixedSelects { get; }
        bool CanRenderOneventAndPrevElementsTogether { get; }
        bool CanRenderPostBackCards { get; }
        bool CanRenderSetvarZeroWithMultiSelectionList { get; }
        bool CanSendMail { get; }
        System.Collections.IDictionary Capabilities { get; set; }
        bool CDF { get; }
        Version ClrVersion { get; }
        int CompareFilters(string filter1, string filter2);
        bool Cookies { get; }
        bool Crawler { get; }
       
        int DefaultSubmitButtonLimit { get; }
        void DisableOptimizedCacheKey();
        Version EcmaScriptVersion { get; }
        bool EvaluateFilter(string filterName);
        bool Frames { get; }
        int GatewayMajorVersion { get; }
        double GatewayMinorVersion { get; }
        string GatewayVersion { get; }
        Version[] GetClrVersions();
        bool HasBackButton { get; }
        bool HidesRightAlignedMultiselectScrollbars { get; }
        string HtmlTextWriter { get; set; }
        string Id { get; }
        string InputType { get; }
        bool IsBrowser(string browserName);
        bool IsColor { get; }
        bool IsMobileDevice { get; }
        bool JavaApplets { get; }
        Version JScriptVersion { get; }
        int MajorVersion { get; }
        int MaximumHrefLength { get; }
        int MaximumRenderedPageSize { get; }
        int MaximumSoftkeyLabelLength { get; }
        double MinorVersion { get; }
        string MinorVersionString { get; }
        string MobileDeviceManufacturer { get; }
        string MobileDeviceModel { get; }
        Version MSDomVersion { get; }
        int NumberOfSoftkeys { get; }
        string Platform { get; }
        string PreferredImageMime { get; }
        string PreferredRenderingMime { get; }
        string PreferredRenderingType { get; }
        string PreferredRequestEncoding { get; }
        string PreferredResponseEncoding { get; }
        bool RendersBreakBeforeWmlSelectAndInput { get; }
        bool RendersBreaksAfterHtmlLists { get; }
        bool RendersBreaksAfterWmlAnchor { get; }
        bool RendersBreaksAfterWmlInput { get; }
        bool RendersWmlDoAcceptsInline { get; }
        bool RendersWmlSelectsAsMenuCards { get; }
        string RequiredMetaTagNameValue { get; }
        bool RequiresAttributeColonSubstitution { get; }
        bool RequiresContentTypeMetaTag { get; }
        bool RequiresControlStateInSession { get; }
        bool RequiresDBCSCharacter { get; }
        bool RequiresHtmlAdaptiveErrorReporting { get; }
        bool RequiresLeadingPageBreak { get; }
        bool RequiresNoBreakInFormatting { get; }
        bool RequiresOutputOptimization { get; }
        bool RequiresPhoneNumbersAsPlainText { get; }
        bool RequiresSpecialViewStateEncoding { get; }
        bool RequiresUniqueFilePathSuffix { get; }
        bool RequiresUniqueHtmlCheckboxNames { get; }
        bool RequiresUniqueHtmlInputNames { get; }
        bool RequiresUrlEncodedPostfieldValues { get; }
        int ScreenBitDepth { get; }
        int ScreenCharactersHeight { get; }
        int ScreenCharactersWidth { get; }
        int ScreenPixelsHeight { get; }
        int ScreenPixelsWidth { get; }
        bool SupportsAccesskeyAttribute { get; }
        bool SupportsBodyColor { get; }
        bool SupportsBold { get; }
        bool SupportsCacheControlMetaTag { get; }
        bool SupportsCallback { get; }
        bool SupportsCss { get; }
        bool SupportsDivAlign { get; }
        bool SupportsDivNoWrap { get; }
        bool SupportsEmptyStringInCookieValue { get; }
        bool SupportsFontColor { get; }
        bool SupportsFontName { get; }
        bool SupportsFontSize { get; }
        bool SupportsImageSubmit { get; }
        bool SupportsIModeSymbols { get; }
        bool SupportsInputIStyle { get; }
        bool SupportsInputMode { get; }
        bool SupportsItalic { get; }
        bool SupportsJPhoneMultiMediaAttributes { get; }
        bool SupportsJPhoneSymbols { get; }
        bool SupportsQueryStringInFormAction { get; }
        bool SupportsRedirectWithCookie { get; }
        bool SupportsSelectMultiple { get; }
        bool SupportsUncheck { get; }
        bool SupportsXmlHttp { get; }
        bool Tables { get; }
        Type TagWriter { get; }
        string this[string key] { get; }
        string Type { get; }
        bool UseOptimizedCacheKey { get; }
        bool VBScript { get; }
        string Version { get; }
        Version W3CDomVersion { get; }
        bool Win16 { get; }
        bool Win32 { get; }

    }
}

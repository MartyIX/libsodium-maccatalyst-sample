using System.Runtime.InteropServices;
using System.Text;

namespace libsodium_maccatalyst_sample;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

    [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
    private static extern int crypto_aead_aes256gcm_is_available();

    [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
    private static extern void randombytes_buf(out ulong buf, UIntPtr size);

    [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sodium_init();

    [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sodium_library_version_major();

    [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sodium_library_minimal();

    [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sodium_library_version_minor();

    [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr sodium_version_string();

    public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
        StringBuilder sb = new();

        try
        {
            sb.AppendFormat("sodium_version_string: {0}; ", Marshal.PtrToStringAnsi(sodium_version_string()));
            sb.AppendFormat("sodium_library_version_major: {0}; ", sodium_library_version_major());
            sb.AppendFormat("sodium_library_version_minor: {0}; ", sodium_library_version_minor());
            sb.AppendFormat("sodium_library_minimal: {0}; ", sodium_library_minimal());
            int error = sodium_init();
            sb.AppendFormat("sodium_init: {0}; ", error);
            if (error == 0)
            {
                randombytes_buf(out ulong buf, (UIntPtr)sizeof(ulong));
                sb.AppendFormat("randombytes_buf: 0x'{0:X8}'; ", buf);
                sb.AppendFormat("crypto_aead_aes256gcm_is_available: {0}; ", crypto_aead_aes256gcm_is_available());
            }
        }
        catch (Exception e)
        {
            sb.AppendLine(e.StackTrace);
        }

        // create a new window instance based on the screen size
        Window = new UIWindow (UIScreen.MainScreen.Bounds);

        // create a UIViewController with a single UILabel
        var vc = new UIViewController();
        vc.View!.AddSubview(new UILabel(Window!.Frame)
        {
            BackgroundColor = UIColor.SystemBackground,
            TextAlignment = UITextAlignment.Center,
            LineBreakMode = UILineBreakMode.WordWrap,
            Lines = 0,
            Text = sb.ToString(),
            AutoresizingMask = UIViewAutoresizing.All,
        });
        Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}
}


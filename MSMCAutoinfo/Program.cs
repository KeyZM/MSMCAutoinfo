using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;
using System.Threading;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
//using System.IO;
//using OpenQA.Selenium.DevTools.V131.FedCm;
//using OpenQA.Selenium.Internal.Logging;
//using OpenQA.Selenium.Support.UI;

class Program
{
    public static void AutoLogin_Th_Th()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[AutoLogin_Th] Run AutoLogin\n");
        Console.ForegroundColor = ConsoleColor.White;
        URL.AutoLoginURL = Console.ReadLine();
        if (URL.AutoLoginURL.CheckURLValid())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[AutoLogin_Th] Use AutoLogin : " + URL.AutoLoginURL + "\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[AutoLogin_Th] Not Use AutoLogin\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    static void Main()
    {
        XMethod method = new XMethod();
        ThreadStart AutoLogin_Th_Th_Th = new ThreadStart(AutoLogin_Th_Th);
        Thread childThread = new Thread(AutoLogin_Th_Th_Th);
    //输入邮箱+判断
    InputMail:
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("[XGP Input] XGP input, like <email----password> Or <email:password>");
        Console.ForegroundColor = ConsoleColor.White;
        string AccInfo = Console.ReadLine();
        string[] AccStore = Regex.Split(AccInfo, "----", RegexOptions.IgnoreCase);
        string[] AccStore_Dahai = Regex.Split(AccInfo, ":", RegexOptions.IgnoreCase);
        if (AccStore.Length != 2)
        {
            if (AccStore_Dahai.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error XGP,Re-input\n");
                Console.ForegroundColor = ConsoleColor.White;
                goto InputMail;
            }
            else
            {
                AccStore = AccStore_Dahai;
            }
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[XGP Input] Email: " + AccStore[0] + "\n[XGP Input] Passwd: " + AccStore[1] + "\n");
        Console.ForegroundColor = ConsoleColor.White;

        //GetPlayerName
        FileStream LoaclConfig = new FileStream("MSAutoInfo.cfg", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamReader reader = new StreamReader(LoaclConfig, leaveOpen: true);
        method.PlayerName = reader.ReadToEnd();
        reader.Close();
        LoaclConfig.Position = 0;
    PlayerNameDec:
        if (string.IsNullOrEmpty(method.PlayerName))
        {
            StreamWriter writer = new StreamWriter(LoaclConfig, leaveOpen: true);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Input PlayerName");
            Console.ForegroundColor = ConsoleColor.White;
            method.PlayerName = Console.ReadLine();
            writer.Write(method.PlayerName);
            writer.Close();
            goto PlayerNameDec;
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[XGP Input] Use PlayerName: " + method.PlayerName + "\n");
        Console.ForegroundColor = ConsoleColor.White;
        LoaclConfig.Close();
        method.RandomLength = (15 - method.PlayerName.Length);
        method.OldPlayerName = method.PlayerName;
        method.PlayerName = method.PlayerName + "_" + method.GetRandomName(method.RandomLength, true, true, true, false, "");

    //SkinPath
    SkinPath:
        if (!File.Exists(System.IO.Path.GetFullPath("skin.png")))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: No skin.png\nPlz add skin.png in MSMCAutoinfo\n");
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(5000);
            goto SkinPath;
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[SkinEditer] Skin path:" + System.IO.Path.GetFullPath("skin.png") + "\n");
        Console.ForegroundColor = ConsoleColor.White;

        new DriverManager().SetUpDriver(new ChromeConfig());
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Loading/Downloading WebDriver,plz wait...\n");
        Console.ForegroundColor = ConsoleColor.White;

        //Options
        var ClientOption = new ChromeOptions();
        ClientOption.AddArgument("window-size=1920,1080");
        ClientOption.AddArgument("--incognito");
        //ClientOption.AddArgument("--headless");

        //StartClient
        IWebDriver WebClient = new ChromeDriver(ClientOption);

        //判断是否使用Autologin
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("[AutoLogin Input] Use AutoLogin?\n[AutoLogin Input] if use , input LoginURL , Like <https://login.live.com/...>\n[AutoLogin Input] if not use , enter");
        Console.ForegroundColor = ConsoleColor.White;
        childThread.Start();


    ReLoadURL:
        WebClient.Url = "https://www.minecraft.net/zh-hans/msaprofile/mygames/editprofile";

        Console.WriteLine("\n");
        try
        {
            WebClient.FindElement(By.ClassName("MC_modal_close")).Click();
        }
        catch (OpenQA.Selenium.ElementNotInteractableException e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n[163JumpOut] Error" + e.Message + "\n[163JumpOut] Skipped");
            Console.ForegroundColor = ConsoleColor.White;
        }
        catch(OpenQA.Selenium.NoSuchElementException e1)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n[163JumpOut] Error" + e1.Message + "\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

    LoginBTN:
        Thread.Sleep(500);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[Login] Get LoginBtn\n");
        Console.ForegroundColor = ConsoleColor.White;
        try
        {
            WebClient.FindElement(By.ClassName("mb-4")).Click();
        }
        catch (OpenQA.Selenium.NoSuchElementException e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Clock Detector] Error: " + e.Message);
            Console.ForegroundColor = ConsoleColor.White;
            goto LoginBTN;
        }

        //Input Email
        Thread.Sleep(300);
        try
        {
            WebClient.FindElement(By.Name("loginfmt")).SendKeys(AccStore[0]);
        }
        catch (OpenQA.Selenium.NoSuchElementException e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n[Method Detector] Error: " + e.Message + "\n");
            Console.ForegroundColor = ConsoleColor.White;
            method.NewMethod = true;
            method.MethodVersion = "Old";
            try
            {
                 WebClient.FindElement(By.Id("usernameEntry")).SendKeys(AccStore[0]);
            }
            catch (OpenQA.Selenium.NoSuchElementException e1)
            {
                Console.WriteLine("[Login] ReLoad because error: \n" + e1.Message);
                goto ReLoadURL;
            }

        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[Method Detector] Use " + method.MethodVersion + "Method\n\n[Login] Input" + method.MethodVersion + "\n");
        Console.ForegroundColor = ConsoleColor.White;

        //next
        if (method.NewMethod)
        {
            WebClient.FindElement(By.XPath("//button[contains(@type,\"submit\")]")).Click();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Login] ButtonNew\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            WebClient.FindElement(By.Id("idSIButton9")).Click();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Login] ButtonOld\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

    //Input Password
    InputPasswd:
        Thread.Sleep(1000);
        if (method.NewMethod)
        {
            try
            {
                WebClient.FindElement(By.Id("passwordEntry")).SendKeys(AccStore[1]);
            }
            catch(OpenQA.Selenium.NoSuchElementException e)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[Clock Detector] Add SleepTime 0.5s To Detect Email\n");
                Console.ForegroundColor = ConsoleColor.White;
                Thread.Sleep(500);
                try
                {
                    WebClient.FindElement(By.Id("passwordEntry")).SendKeys(AccStore[1]);
                }
                catch(OpenQA.Selenium.NoSuchElementException e1)
                {
                    if (method.IswrongEmail == 3)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[Login] Error Email\n\nDone,press enter to exit");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.ReadLine();
                        return;
                    }
                    ++method.IswrongEmail;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[Clock Detector] Wrong: Add SleepTime 0.5s To Detect Email\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    goto InputPasswd;
                }
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Login] InputNew\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            try
            {
                WebClient.FindElement(By.Name("passwd")).SendKeys(AccStore[1]);
            }
            catch(OpenQA.Selenium.NoSuchElementException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Login] Wrong Email");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Done,press enter to exit");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadLine();
                return;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Login] InputOld\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        //Login
        if (method.NewMethod)
        {
            WebClient.FindElement(By.XPath("//button[contains(@type,\"submit\")]")).Click();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Login] ButtonNew\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            WebClient.FindElement(By.Id("idSIButton9")).Click();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Login] ButtonOld\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        //iShowSkip
        Thread.Sleep(800);
        for (int i = 0; i < 5; i++)
        {
            try
            {
                WebClient.FindElement(By.Id("iShowSkip")).Click();
            }
            catch (OpenQA.Selenium.NoSuchElementException e)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[ShowSkiper] Error: " + e.Message + "\n");
                Console.ForegroundColor = ConsoleColor.White;
                break;
            }
        }

    //LoginStats
    LoginState:
        Thread.Sleep(500);
        try
        {
            WebClient.FindElement(By.Id("acceptButton")).Click();
        }
        catch (OpenQA.Selenium.NoSuchElementException e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Clock Detector] Error: " + e.Message);
            Console.ForegroundColor = ConsoleColor.White;
            goto LoginState;
        }

        //163JumpOut
        Thread.Sleep(1000);
        WebClient.Url = "https://www.minecraft.net/zh-hans/msaprofile/mygames/editprofile";
        try
        {
            WebClient.FindElement(By.ClassName("MC_modal_close")).Click();
        }
        catch (OpenQA.Selenium.ElementNotInteractableException e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n[163JumpOut] Error" + e.Message+ "\nSkipped");
            Console.ForegroundColor = ConsoleColor.White;
        }
        catch(OpenQA.Selenium.NoSuchElementException e1)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n[163JumpOut] Error" + e1.Message + "\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        //InputPlayerName
        //method.PlayerName = "Xiaolin0721_CEzC";//敏感词
        //method.PlayerName = "Xiaolin0721_7l6N";//重复
    SetNameError:
        Thread.Sleep(2000);
        try
        {
            WebClient.FindElement(By.Id("change-java-profile-name")).Clear();
        }
        catch (OpenQA.Selenium.NoSuchElementException e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[SetName] Error: " + e.Message + "\n");
            Console.ForegroundColor = ConsoleColor.White;
            try
            {
                WebClient.FindElement(By.ClassName("mb-4")).Click();
            }
            catch (OpenQA.Selenium.NoSuchElementException e1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[SetName] Error: " + e1.Message + "\n");
                Console.ForegroundColor = ConsoleColor.White;
                goto SetNameError;
            }
            goto SetNameError;
        }
        catch (OpenQA.Selenium.InvalidElementStateException e1)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[SetName] Error: " + e1.Message + "\n");
            Console.ForegroundColor = ConsoleColor.White;
            goto SetSkin;
        }
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Now-PlayerName: " + method.PlayerName + "\n");
        Console.ForegroundColor = ConsoleColor.White;

        WebClient.FindElement(By.Id("change-java-profile-name")).SendKeys(method.PlayerName);
        try
        {
            WebClient.FindElement(By.XPath("//button[contains(@class,\"MC_Button_Primary\")]")).Click();
        }
        catch(OpenQA.Selenium.ElementClickInterceptedException e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Clock Detector] Error: " + e.Message);
            Console.ForegroundColor = ConsoleColor.White;
            goto SetNameError;
        }
        Thread.Sleep(2000);

    SetSkin:
        WebClient.Url = "https://www.minecraft.net/zh-hans/msaprofile/mygames/editskin";
        try
        {
            WebClient.FindElement(By.ClassName("MC_modal_close")).Click();
        }
        catch (OpenQA.Selenium.ElementNotInteractableException e)
        {
            Console.WriteLine("\n[163JumpOut] Error" + e.Message + "\nSkipped");
        }
        catch(OpenQA.Selenium.NoSuchElementException e1)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n[163JumpOut] Error" + e1.Message + "\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
        Thread.Sleep(3000);
        try
        {
            WebClient.FindElement(By.XPath("//input[contains(@type,\"file\")]")).SendKeys(System.IO.Path.GetFullPath("skin.png"));
        }
        catch(OpenQA.Selenium.WebDriverArgumentException e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[SkinEditer] Error: " + e.Message + "\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
        catch(OpenQA.Selenium.NoSuchElementException)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[XGP Input] is Change PlayerName Times: " + method.IsPlayerNameChange + "\n");
            Console.ForegroundColor = ConsoleColor.White;
            ++method.IsPlayerNameChange;
            if (method.IsPlayerNameChange == 2)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[XGP Input] Changed XGPName\n");
                Console.ForegroundColor = ConsoleColor.White;
                method.PlayerName = method.OldPlayerName + "_" + method.GetRandomName(method.RandomLength, true, true, true, false, "");
            }
            goto SetNameError;
        }

        WebClient.FindElement(By.XPath("//button[contains(@class,\"MC_Button MC_Button_Secondary MC_Style_Green_5\")]")).Click();

        //AutoLogin
    AutoLoginMethod:
        if (URL.AutoLoginURL.CheckURLValid())
        {
            Thread.Sleep(800);
            WebClient.Url = URL.AutoLoginURL;
            Thread.Sleep(800);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[AutoLogin] AutoLogin\n");
            Console.ForegroundColor = ConsoleColor.White;
            try
            {
                WebClient.FindElement(By.Id("idBtn_Accept")).Click();
            }
            catch(OpenQA.Selenium.NoSuchElementException e)
            {
                //Myau
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[AutoLogin] Error: " + e.Message + "\n");
                Console.ForegroundColor = ConsoleColor.White;
                try
                {
                    string xpath = "//button[//div[contains(text(),'@outlook.com')]]";
                    WebClient.FindElement(By.XPath(xpath)).Click();
                }
                catch(OpenQA.Selenium.NoSuchElementException e1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[AutoLogin] Error: " + e1.Message + "\n");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Thread.Sleep(500);
            LoginClockAdd:
                Thread.Sleep(500);
                try
                {
                    WebClient.FindElement(By.XPath("//button[contains(@data-testid,\"appConsentPrimaryButton\")]")).Click();
                }
                catch (OpenQA.Selenium.NoSuchElementException e1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[Clock Detector] Error: " + e1 + "\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    try
                    {
                        WebClient.FindElement(By.Id("//input[contains(@class,\"btn btn-block btn-primary\")]")).Click();
                    }
                    catch(OpenQA.Selenium.NoSuchElementException e2)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("[Clock Detector] Error: " + e2 + "\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        goto LoginClockAdd;
                    }
                }
            }
        }

        Console.ForegroundColor= ConsoleColor.Blue;
        Console.WriteLine("Done,press enter to exit");
        Console.ForegroundColor = ConsoleColor.White;
        Console.ReadLine();
        WebClient.Quit();
    }
}

class XMethod
{
    public bool NewMethod = false;
    public string MethodVersion = "New";
    public string OldPlayerName;
    public string PlayerName;
    public int IswrongEmail = 0;
    public int IsPlayerNameChange = 1;
    public int RandomLength;
    public string GetRandomName(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
    {
        byte[] b = new byte[4];
        new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
        Random r = new Random(BitConverter.ToInt32(b, 0));
        string s = null, str = custom;
        if (useNum == true)
        { str += "0123456789"; }
        if (useLow == true)
        { str += "abcdefghijklmnopqrstuvwxyz"; }
        if (useUpp == true)
        { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
        if (useSpe == true)
        { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
        for (int i = 0; i < length; i++)
        {
            s += str.Substring(r.Next(0, str.Length - 1), 1);
        }
        return s;
    }
}
static class URLValidator
{
    public static bool CheckURLValid(this string source)
    {
        Uri uriResult;
        return Uri.TryCreate(source, UriKind.Absolute, out uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
static class URL
{
    public static string AutoLoginURL = "NoLogin";
}

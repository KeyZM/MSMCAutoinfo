using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;
//using System.Threading;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
//using System.IO;
//using OpenQA.Selenium.DevTools.V131.FedCm;
//using OpenQA.Selenium.Internal.Logging;
//using OpenQA.Selenium.Support.UI;

class Program
{
    public static void AutoLogin_Thread()
    {
        Output.OutputBlue("[AutoLogin Input] Use AutoLogin?\n[AutoLogin Input] if use , input LoginURL , Like <https://login.live.com/...>\n");
        URL.AutoLoginURL = Console.ReadLine();
        if (URL.AutoLoginURL.CheckURLValid())
        {
            Output.OutputBlue("[AutoLogin] Use AutoLogin\n");
        }
        else
        {
            Output.OutputYellow("[AutoLogin] Not Use AutoLogin\n");
        }
    }

    static void Main()
    {
        XMethod method = new XMethod();
        ThreadStart AutoLogin_ThreadStarter = new ThreadStart(AutoLogin_Thread);
        Thread Autologin__Thread = new Thread(AutoLogin_ThreadStarter);
    //输入邮箱+判断
    InputMail:
        Output.OutputBlue("[XGP Input] InPut a XGP, like <email----password> Or <email:password>");
        string AccInfo = Console.ReadLine();
        string[] AccStore = Regex.Split(AccInfo, "----", RegexOptions.IgnoreCase);
        string[] AccStore_Dahai = Regex.Split(AccInfo, ":", RegexOptions.IgnoreCase);
        if (AccStore.Length != 2)
        {
            if (AccStore_Dahai.Length != 2 && AccStore_Dahai.Length !=3) 
            {
                Output.OutputRed("Error XGP,Re-input\n");
                goto InputMail;
            }
            else
            {
                AccStore = AccStore_Dahai;
            }
        }
        //Output.OutputYellow("[XGP Input] Email: " + AccStore[0] + "\n[XGP Input] Passwd: " + AccStore[1] + "\n");
        //GetPlayerName
        FileStream LoaclConfig = new FileStream("MSAutoInfo.cfg", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamReader reader = new StreamReader(LoaclConfig, leaveOpen: true);
        method.configstore = reader.ReadToEnd();
        string[] ConfigInfos = Regex.Split(method.configstore, ";", RegexOptions.IgnoreCase);
        if (ConfigInfos.Length > 1)
        {
            method.OldPlayerName = ConfigInfos[0];
        }
        reader.Close();
        LoaclConfig.Position = 0;
    PlayerNameDec:
        if ((method.OldPlayerName) == "Zayess")
        {
            Output.OutputRed("Input PlayerName");
            method.OldPlayerName = Console.ReadLine();
            goto PlayerNameDec;
        }
        Output.OutputYellow("[XGP Input] PlayerName: " + method.OldPlayerName + "\n");

    //SkinPath
    SkinPath:
        if (ConfigInfos.Length > 1)
        {
            if (ConfigInfos[1] == "False")
            {
                method.IsAutoSetSkin = false;
            }
        }else if (method.IsAutoSetSkin == false)
        {
            if (File.Exists(System.IO.Path.GetFullPath("skin.png")))
            {
                method.IsAutoSetSkin = true;
            }
            else
            {
                goto JumpSetSkin;
            }
        }
        if (!(File.Exists(System.IO.Path.GetFullPath("skin.png"))) && method.IsAutoSetSkin == true)
        {
            Output.OutputRed("[SkinEditer] if you want to auto-set-skin,Plz add skin.png -> " + System.IO.Path.GetFullPath("skin.png") + "\nif not plz enter");
            Console.ReadLine();
            method.IsAutoSetSkin = false;
            goto SkinPath;
        }
    //Output.OutputYellow("[SkinEditer] Skin path:" + System.IO.Path.GetFullPath("skin.png") + "\n");
    JumpSetSkin:
        StreamWriter writer = new StreamWriter(LoaclConfig, leaveOpen: true);
        writer.Write(method.OldPlayerName + ";" + method.IsAutoSetSkin);
        writer.Close();
        LoaclConfig.Close();

        new DriverManager().SetUpDriver(new ChromeConfig());
        Output.OutputYellow("Loading...\n");

        //Options
        var ClientOption = new ChromeOptions();
        ClientOption.AddArgument("window-size=1920,1080");
        ClientOption.AddArgument("--incognito");
        //ClientOption.AddArgument("--headless");

        //StartClient
        IWebDriver WebClient = new ChromeDriver(ClientOption);

        //判断是否使用Autologin
        Autologin__Thread.Start();


    ReLoadURL:
        WebClient.Url = "https://www.minecraft.net/zh-hans/msaprofile/mygames/editprofile";
        try
        {
            WebClient.FindElement(By.ClassName("MC_modal_close")).Click();
        }
        catch (OpenQA.Selenium.ElementNotInteractableException e)
        {
            //Output.OutputYellow("\n[163JumpOut] Error" + e.Message + "\n[163JumpOut] Skipped\n");
        }
        catch(OpenQA.Selenium.NoSuchElementException e1)
        {
            //Output.OutputYellow("\n[163JumpOut] Error" + e1.Message + "\n");
        }

    LoginBTN:
        Thread.Sleep(500);
        //Output.OutputYellow("[Login] Get LoginBtn\n");
        try
        {
            WebClient.FindElement(By.ClassName("mb-4")).Click();
        }
        catch (OpenQA.Selenium.NoSuchElementException e)
        {
            //Output.OutputYellow("[Clock Detector] Error: " + e.Message + "\n");
            Output.OutputYellow("[Clock Detector] Wait\n");
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
            //Output.OutputYellow("[Method Detector] OldMethod\n");
            method.NewMethod = true;
            method.MethodVersion = "Old";
            try
            {
                 WebClient.FindElement(By.Id("usernameEntry")).SendKeys(AccStore[0]);
            }
            catch (OpenQA.Selenium.NoSuchElementException e1)
            {
                Output.OutputRed("[Login] ReLoad\n");
                goto ReLoadURL;
            }

        }
        //Output.OutputYellow("[Method Detector] Use " + method.MethodVersion + "Method\n\n[Login] Input" + method.MethodVersion + "\n");

        //next
        if (method.NewMethod)
        {
            WebClient.FindElement(By.XPath("//button[contains(@type,\"submit\")]")).Click();
            //Output.OutputYellow("[Login] ButtonNew\n");
        }
        else
        {
            WebClient.FindElement(By.Id("idSIButton9")).Click();
            //Output.OutputYellow("[Login] ButtonOld\n");
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
                Output.OutputYellow("[Clock Detector] Wait\n");
                Thread.Sleep(500);
                try
                {
                    WebClient.FindElement(By.Id("passwordEntry")).SendKeys(AccStore[1]);
                }
                catch(OpenQA.Selenium.NoSuchElementException e1)
                {
                    if (method.IswrongEmail == 3)
                    {
                        Output.OutputRed("[Login] Error Email\n\nDone,press enter to exit");
                        Console.ReadLine();
                        return;
                    }
                    ++method.IswrongEmail;
                    Output.OutputYellow("[Clock Detector] Wrong: Add SleepTime 0.5s To Detect Email\n");
                    goto InputPasswd;
                }
            }
            //Output.OutputYellow("[Login] InputNew\n");
        }
        else
        {
            try
            {
                WebClient.FindElement(By.Name("passwd")).SendKeys(AccStore[1]);
            }
            catch(OpenQA.Selenium.NoSuchElementException e)
            {
                Output.OutputRed("[Login] Wrong Email\nDone,enter to exit");
                Console.ReadLine();
                return;
            }
            //Output.OutputYellow("[Login] InputOld\n");
        }

        //Login
        if (method.NewMethod)
        {
            WebClient.FindElement(By.XPath("//button[contains(@type,\"submit\")]")).Click();
            //Output.OutputYellow("[Login] ButtonNew\n");
        }
        else
        {
            WebClient.FindElement(By.Id("idSIButton9")).Click();
            //Output.OutputYellow("[Login] ButtonOld\n");
        }

    //iShowSkip
    IShowSkip:
        Thread.Sleep(800);
        for (int i = 0; i < 5; i++)
        {
            try
            {
                WebClient.FindElement(By.Id("iShowSkip")).Click();
            }
            catch (OpenQA.Selenium.NoSuchElementException e)
            {
                //Output.OutputYellow("[ShowSkiper] Error: " + e.Message + "\n");
                break;
            }
            catch (OpenQA.Selenium.StaleElementReferenceException e1)
            {
                Output.OutputYellow("[Clock Detector] Wait\n");
                goto IShowSkip;
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
            Output.OutputYellow("[Clock Detector] Wait\n");
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
            //Output.OutputYellow("[163JumpOut] Error" + e.Message + "\n");
        }
        catch(OpenQA.Selenium.NoSuchElementException e1)
        {
            //Output.OutputYellow("\n[163JumpOut] Error" + e1.Message + "\n");

        }

        //InputPlayerName
        //method.PlayerName = "Xiaolin0721_CEzC";//敏感词
        //method.PlayerName = "Xiaolin0721_7l6N";//重复
        method.PlayerName = method.OldPlayerName + "_" + method.GetRandomName((15 - method.OldPlayerName.Length), true, true, true, false, "");
    SetNameError:
        Thread.Sleep(2000);
        try
        {
            WebClient.FindElement(By.Id("change-java-profile-name")).Clear();
        }
        catch (OpenQA.Selenium.NoSuchElementException e)
        {
            //Output.OutputYellow("[SetName] Error\n");
            try
            {
                WebClient.FindElement(By.ClassName("mb-4")).Click();
            }
            catch (OpenQA.Selenium.NoSuchElementException e1)
            {
                Output.OutputYellow("[SetName] Wait\n");
                goto SetNameError;
            }
            goto SetNameError;
        }
        catch (OpenQA.Selenium.InvalidElementStateException e1)
        {
            Output.OutputYellow("[SetSkin] Wait\n");
            goto SetSkin;
        }
        Output.OutputBlue("Now-PlayerName: " + method.PlayerName + "\n");

        WebClient.FindElement(By.Id("change-java-profile-name")).SendKeys(method.PlayerName);
        try
        {
            WebClient.FindElement(By.XPath("//button[contains(@class,\"MC_Button_Primary\")]")).Click();
        }
        catch(OpenQA.Selenium.ElementClickInterceptedException e)
        {
            Output.OutputYellow("[Clock Detector] Wait\n");
            goto SetNameError;
        }
        Thread.Sleep(2000);

    SetSkin:
        if (method.IsAutoSetSkin == false)
        {
            //Output.OutputYellow("[SkinEditer] Skip\n");
            goto AutoLoginMethod;
        }
        WebClient.Url = "https://www.minecraft.net/zh-hans/msaprofile/mygames/editskin";
        try
        {
            WebClient.FindElement(By.ClassName("MC_modal_close")).Click();
        }
        catch (OpenQA.Selenium.ElementNotInteractableException e)
        {
            //Output.OutputYellow("\n[163JumpOut] Error" + e.Message + "\nSkipped");
        }
        catch(OpenQA.Selenium.NoSuchElementException e1)
        {
            //Output.OutputYellow("\n[163JumpOut] Error" + e1.Message + "\n");
        }
        Thread.Sleep(3000);
        try
        {
            WebClient.FindElement(By.XPath("//input[contains(@type,\"file\")]")).SendKeys(System.IO.Path.GetFullPath("skin.png"));
        }
        catch(OpenQA.Selenium.WebDriverArgumentException e)
        {
            //Output.OutputYellow("[SkinEditer] Error\n");
        }
        catch(OpenQA.Selenium.NoSuchElementException)
        {
            Output.OutputYellow("[XGP Input] Change PlayerName " + method.IsPlayerNameChange + "\n");
            ++method.IsPlayerNameChange;
            if (method.IsPlayerNameChange == 2)
            {
                Output.OutputYellow("[XGP Input] Changed XGPName\n");
                method.PlayerName = method.OldPlayerName + "_" + method.GetRandomName((15 - method.OldPlayerName.Length), true, true, true, false, "");
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
            Output.OutputYellow("[AutoLogin] AutoLogin\n");
            try
            {
                WebClient.FindElement(By.Id("idBtn_Accept")).Click();
            }
            catch(OpenQA.Selenium.NoSuchElementException e)
            {
                //Myau
                Output.OutputYellow("[AutoLogin] Error: " + e.Message + "\n");
                try
                {
                    string xpath = "//button[//div[contains(text(),'@outlook.com')]]";
                    WebClient.FindElement(By.XPath(xpath)).Click();
                }
                catch(OpenQA.Selenium.NoSuchElementException e1)
                {
                    Output.OutputYellow("[AutoLogin] Error: " + e1.Message + "\n");
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
                    Output.OutputYellow("[Clock Detector] Error: " + e1 + "\n");
                    try
                    {
                        WebClient.FindElement(By.Id("//input[contains(@class,\"btn btn-block btn-primary\")]")).Click();
                    }
                    catch(OpenQA.Selenium.NoSuchElementException e2)
                    {
                        //Output.OutputYellow("[Clock Detector] Wait\n");
                    }
                }
            }
        }
        Output.OutputBlue("Done,enter to exit");
        Console.ReadLine();
        WebClient.Quit();
    }
}

class XMethod
{
    public bool IsAutoSetSkin = true;
    public bool NewMethod = false;
    public string MethodVersion = "New";
    public string configstore;
    public string OldPlayerName = "Zayess";
    public string PlayerName;
    public int IswrongEmail = 0;
    public int IsPlayerNameChange = 1;
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
static class URL
{
    public static string AutoLoginURL = "NoLogin";
    public static bool CheckURLValid(this string source)
    {
        Uri uriResult;
        return Uri.TryCreate(source, UriKind.Absolute, out uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
static class Output
{
    public static void OutputRed(string Msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(Msg);
        Console.ForegroundColor = ConsoleColor.White;
    }
    public static void OutputBlue(string Msg)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(Msg);
        Console.ForegroundColor = ConsoleColor.White;
    }
    public static void OutputYellow(string Msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(Msg);
        Console.ForegroundColor = ConsoleColor.White;
    }
}
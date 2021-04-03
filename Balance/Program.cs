using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

#if DEBUG
using System.Runtime.InteropServices;
#endif

namespace Balance
{
    static class Program
    {
        public static string db_path;
        public static string passwd_path;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            AllocConsole();
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            db_path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "data.sqlite");
            passwd_path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "passwd.json");

            object task = new Task<byte[]>(() => new byte[100]);
            Console.WriteLine((task.GetType() == typeof(Task<byte[]>)));
#if DEBUG
            Console.WriteLine(Path.GetTempPath());
#endif
            
            if (!File.Exists(db_path) || !File.Exists(passwd_path))
            {
                SecureSQLiteContext ctx = SecureSQLiteContext.FirstRun(db_path);
                AuthStorage storage = new AuthStorage();
                CreateNewUserForm newform = new CreateNewUserForm(ctx, storage);
                if(newform.ShowDialog() == DialogResult.OK)
                {
                    storage.serialize(passwd_path);
                    Task tsk = ctx.Unload();
                    tsk.Wait();
                    Run();
                } 
                else
                {
                    if (File.Exists(db_path)) File.Delete(db_path);
                    if (File.Exists(passwd_path)) File.Delete(passwd_path);
                }
            }
            else
            {
                Run();
            }
        }
        static void Run()
        {
            LoginForm loginForm = new LoginForm();
            if(loginForm.ShowDialog() == DialogResult.OK)
            {
#if DEBUG
                Console.WriteLine("Автентифiкацiя успiшна!");
#endif
                MainWindow main = new MainWindow(loginForm.Context);
                Application.Run(main);
            }
            else
            {
#if DEBUG
                Console.WriteLine("Автентифiкацiю вiдмiнив користувач");
#endif
                MessageBox.Show("Автентифiкацiю вiдмiнив користувач");
                
                if(loginForm.Context != null)
                    loginForm.Context.Unload().Wait();
                
            }
        }
#if DEBUG
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
#endif

    }
}

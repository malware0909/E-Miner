using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace m.Properties
{
	// Token: 0x02000007 RID: 7
	[DebuggerNonUserCode]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[CompilerGenerated]
	public class Resources
	{
		// Token: 0x06000028 RID: 40 RVA: 0x00002050 File Offset: 0x00000250
		internal Resources()
		{
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002F0C File Offset: 0x0000110C
		public static byte[] AudioHD
		{
			get
			{
				return (byte[])Resources.ResourceManager.GetObject("AudioHD", Resources.resourceCulture);
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002F34 File Offset: 0x00001134
		// (set) Token: 0x0600002B RID: 43 RVA: 0x0000217A File Offset: 0x0000037A
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600002C RID: 44 RVA: 0x00002F48 File Offset: 0x00001148
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("m.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x04000017 RID: 23
		private static CultureInfo resourceCulture;

		// Token: 0x04000018 RID: 24
		private static ResourceManager resourceMan;
	}
}

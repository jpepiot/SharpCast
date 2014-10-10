SharpCast
=========

SharpCast is a C# implementation of the Google ChromeCast V2 protocol.

### USAGE

`Client client = new Client("MYIPADDRESS");`<br/><br/>
`// Get notified when the status of the media changed`<br/>
`client.MediaStatusChanged += (sender, status) => {`<br/>
    `Console.WriteLine("New player state :" + status.PlayerState);`<br/>
`};`<br/><br/>
`client.Connect();`<br/><br/>
`// Load media content`<br/>
`client.Load("Big buck dunny", new Uri("http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4"), "video/mp4", new Uri("http://www.ni-mate.com/wp-content/uploads/2012/12/Very_BBB_xmas1_8bit_600px_wide.jpg"));`<br/>

`// Start playing the media`<br/>
`client.Play();`<br/>

### API

#####.Connect()
#####.Load(`string title, Uri contentUri, string contentType, Uri imageUri, bool autoPlay, StreamType streamType`)
#####.Play()
#####.Pause()
#####.Seek(`double position`)
#####.SetVolume(`double level`)
#####.SetMuted(`bool muted`)
#####.StopApp()
#####.LaunchApp(`string applicationId`)
#####.GetRunningApp()
#####.GetAppAvailability(`string applicationId`)
#####.GetStatus()





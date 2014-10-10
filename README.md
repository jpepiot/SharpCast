SharpCast
=========

SharpCast is a C# implementation of the Google ChromeCast V2 protocol.

### USAGE

`Player player = new Player("MYIPADDRESS");`<br/><br/>
`// Get notified when the status of the media changed`<br/>
`player.MediaStatusChanged += (sender, status) => {`<br/>
    `Console.WriteLine("New player state :" + status.PlayerState);`<br/>
`};`<br/><br/>
`player.Connect();`<br/><br/>
`// Load media content`<br/>
`client.LoadVideo(new Uri("http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4"),`<br/>
        `"video/mp4",` <br/>
        `new MovieMediaMetadata {`<br/>
        `Title = "Big buck dunny",`<br/>
        `Images = new[] { `
            `new Image { `<br/>
            `Url = "http://www.ni-mate.com/wp-content/uploads/2012/12/Very_BBB_xmas1_8bit_600px_wide.jpg" }}});`<br/>

`// Start playing the media`<br/>
`player.Play();`<br/>

### API

#####.Connect()
Connects to ChromeCast device.
#####.Load(`Uri contentUri, string contentType, MediaMetadata metadata, bool autoPlay, StreamType streamType`)
Loads new content.
#####.LoadVideo(`Uri contentUri, string contentType, MovieMediaMetadata metadata, bool autoPlay, StreamType streamType`)
Loads new video content.
#####.LoadPhoto(`Uri contentUri, string contentType, PhotoMediaMetadata metadata, bool autoPlay, StreamType streamType`)
Loads new photo content.
#####.LoadMusic(`Uri contentUri, string contentType, MusicTrackMediaMetadata metadata, bool autoPlay, StreamType streamType`)
Loads new music content.
#####.Play()
Begins playback of the content that was loaded with the load call.
#####.Pause()
Pauses playback of the current content.
#####.Seek(`double position`)
Sets the current position in the stream.
#####.SetVolume(`double level`)
Sets the stream volume.
#####.SetMuted(`bool muted`)
Mutes/Unmutes the stream volume.
#####.StopApp()
Stops playback of the current content.
#####.LaunchApp(`string applicationId`)
#####.GetRunningApp()
Gets running application.
#####.GetAppAvailability(`string applicationId`)
Checks whether an application is available.
#####.GetStatus()
Retrieves the media status.





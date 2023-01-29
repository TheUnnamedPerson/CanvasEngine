#CanvasEngine

Feeds Any Upcoming Assignments on Canvas LMS To Wallpaper Engine

Specify your institution's canvas URL and you Authentication Code in the config files.

Follow [This Guide](https://canvas.instructure.com/doc/api/file.oauth.html#manual-token-generation) To Get Your Auth Token.

In Wallpaper Engine the User Property that is Modified should have the key "assignmentslist" in the editor.

Set Whatever Tex Field You Want Updated to Script and use the following code inside your Update Function:

```var s = engine.userProperties.assignmentslist;
	value = s.replace("\\n", "\n");
	
	return value;
```

For Information on the Canvas LMS API go to https://canvas.instructure.com/doc/api/
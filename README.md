# FolderLinker
在配置文件中添加好需要建立软链接的文件夹路径，程序会自动读取这些路径，并将该文件夹下的所有子文件夹软连接到指定的位置。

应用场景如下：

家里Nas上已经搭建好了Plex媒体服务器，但是媒体文件位于多个磁盘，如果要在WebDAV或者Samba等位置访问到媒体服务器文件并且保持所有文件目录尽然有序，那么软连接是个很好的解决方案。

配置文件定义如下：

```xml
<Config>
  <Linker SourceDirectory="D:\PlexMediaLibrary\Movies" DestinationDirectory="E:\WebDAV\Plex\Movies"/>
  <Linker SourceDirectory="E:\PlexMediaLibrary\Movies" DestinationDirectory="E:\WebDAV\Plex\Movies"/>
  <Linker SourceDirectory="D:\PlexMediaLibrary\TV Shows" DestinationDirectory="E:\WebDAV\Plex\TV Shows"/>
  <Linker SourceDirectory="E:\PlexMediaLibrary\TV Shows" DestinationDirectory="E:\WebDAV\Plex\TV Shows"/>
</Config>
```

程序运行需要.NetCore 3.1运行时。

程序可以监控指定的所有文件夹，当文件夹发生变动时也会自动将目标文件夹做同样的处理。配合WinSW等工具可以将程序封装成后台服务，一劳永逸。

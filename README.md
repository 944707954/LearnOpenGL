### LearnOpenGL

- 这里记录了一个萌新学习OpenGL的过程

- 中文网站： https://learnopengl-cn.github.io/ 

- 英文网站： https://learnopengl.com/ 

- 我的代码地址： https://github.com/944707954/LearnOpenGL 

- 我的VS配置：

  1. Include Directories(包含目录)：$(ProjectDir)\Libraries\include

  2. lib Directories(库目录)：$(ProjectDir)\Libraries\lib

  3. Linker(链接器)-Input(输入)-Additional Dependencies(附加依赖项)：

     opengl32.lib;

     glfw3.lib;

     assimp-vc142-mtd.lib

  4. 若使用assimp，需添加 在目录中添加assimp-vc142-mtd.dll 

  5. 添加glad.c到文件夹，同时加入工程

- 目录


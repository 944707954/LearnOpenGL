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

  - 1_HelloWindow
  - 2_1_LightingMaps
  - 2_2_LightCaster
  - 3_Assimp
  - 4_1_StencilTesting
  - 4_2_Blending
  - 4_2_Blending_Faceculling
  - 4_3_FrameBuffer
  - 4_4_Skybox
  - 4_5_AdvancedGLSL
  - 4_6_GeometryShader
  - 4_7_Instancing
  - 4_8_AntiAliasing
  - 5_1_BlinnPhong
  - 5_2_GammaCorrection
  - 5_3_ShadowMapping


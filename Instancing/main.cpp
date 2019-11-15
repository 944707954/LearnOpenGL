#include <glad/glad.h>
#include <GLFW/glfw3.h>

#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>


#include <learnopengl/shader.h>
#include <learnopengl/camera.h>
#include <learnopengl/model.h>
#include <string>

#include <iostream>


void processInput(GLFWwindow* window);
void framebuffer_size_callback(GLFWwindow* window, int width, int height);
void mouse_callback(GLFWwindow* window, double xpos, double ypos);
void scroll_callback(GLFWwindow* window, double xoffset, double yoffset);

const unsigned int SCR_WIDTH = 1280;
const unsigned int SCR_HEIGHT = 720;


// camera
Camera camera(glm::vec3(0.0f, 30.0f, 255.0f));
float lastX = SCR_WIDTH / 2.0f;
float lastY = SCR_HEIGHT / 2.0f;
bool firstMouse = true;

// timing
float deltaTime = 0.0f;	// time between current frame and last frame
float lastFrame = 0.0f;

int main()
{
	glfwInit();
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
	//glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);  //Mac OS X

	GLFWwindow* window = glfwCreateWindow(SCR_WIDTH, SCR_HEIGHT, "window", NULL, NULL);
	
	if (window == NULL)
	{
		std::cout << "Failed to create window.\n" << std::endl;
		glfwTerminate();
		return -1;
	}
	glfwMakeContextCurrent(window);
	glfwSetFramebufferSizeCallback(window, framebuffer_size_callback);
	glfwSetCursorPosCallback(window, mouse_callback);
	glfwSetScrollCallback(window, scroll_callback);

	// tell GLFW to capture our mouse
	glfwSetInputMode(window, GLFW_CURSOR, GLFW_CURSOR_DISABLED);


	if (!gladLoadGLLoader((GLADloadproc)glfwGetProcAddress))
	{
		std::cout << "Failed to init GLAD.\n" << std::endl;
		return -1;
	}

	glEnable(GL_DEPTH_TEST);

	Shader planetShader("shaders/planet.vs", "shaders/planet.fs");
	Shader rockShader("shaders/rock.vs", "shaders/rock.fs");
	Model planetModel("planet/planet.obj");
	Model rockModel("rock/rock.obj");

	//float quadVertices[] = {
	//	// positions     // colors
	//	-0.05f,  0.05f,  1.0f, 0.0f, 0.0f,
	//	 0.05f, -0.05f,  0.0f, 1.0f, 0.0f,
	//	-0.05f, -0.05f,  0.0f, 0.0f, 1.0f,

	//	-0.05f,  0.05f,  1.0f, 0.0f, 0.0f,
	//	 0.05f, -0.05f,  0.0f, 1.0f, 0.0f,
	//	 0.05f,  0.05f,  0.0f, 1.0f, 1.0f
	//};

	//glm::vec2 offset[100];
	//int index = 0;
	//for (int y = -10; y < 10; y += 2)
	//{
	//	for (int x = -10; x < 10; x += 2)
	//	{
	//		glm::vec2 off;
	//		off.x = float(x) / 10.0f + 0.1f;
	//		off.y = float(y) / 10.0f + 0.1f;
	//		offset[index++] = off;
	//	}
	//}

	//unsigned int vao, vbo;

	//glGenVertexArrays(1, &vao);
	//glBindVertexArray(vao);

	//glGenBuffers(1, &vbo);
	//glBindBuffer(GL_ARRAY_BUFFER, vbo);
	//glBufferData(GL_ARRAY_BUFFER, sizeof(quadVertices), quadVertices, GL_STATIC_DRAW);
	//glEnableVertexAttribArray(0);
	//glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(float), (void*)0);
	//glEnableVertexAttribArray(1);
	//glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(float), (void*)(2 * sizeof(float)));

	//unsigned int instanceVBO;
	//glGenBuffers(1, &instanceVBO);
	//glBindBuffer(GL_ARRAY_BUFFER, instanceVBO);
	//glBufferData(GL_ARRAY_BUFFER, sizeof(offset), offset, GL_STATIC_DRAW);
	//glEnableVertexAttribArray(2);
	//glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), (void*)0);
	//glVertexAttribDivisor(2, 1);
	//glBindVertexArray(0);

	
	/*defaultShader.useProgram();
	for (int i = 0; i < 100; i++)
	{
		string offi = "offset[" + to_string(i) + "]";
		defaultShader.setVec2(offi, offset[i]);
	}*/

	unsigned int num = 100000;
	glm::mat4* modelMatrix = new glm::mat4[num];
	srand(glfwGetTime());
	float radius = 150.0;
	float offset = 25.0;
	for (int i = 0; i < num; i++)
	{
		//translate
		glm::mat4 model = glm::mat4(1.0);
		float angle = float(i) / float(num) * 360.0;
		float d = (rand() % int(2 * offset * 100)) / 100.0 - offset;
		float x = sin(angle) * radius + d;
		d = (rand() % int(2 * offset * 100)) / 100.0 - offset;
		float y = d * 0.4;
		d = (rand() % int(2 * offset * 100)) / 100.0 - offset;
		float z = cos(angle) * radius + d;
		model = glm::translate(model, glm::vec3(x, y, z));
		//scale
		float scale = (rand() % 20) / 100.0 + 0.05;
		model = glm::scale(model, glm::vec3(scale));
		//rotation
		float rotAngle = rand() % 360;
		model = glm::rotate(model, rotAngle, glm::vec3(0.4, 0.6, 0.8));

		modelMatrix[i] = model;
	}

	unsigned int buffer;
	glGenBuffers(1, &buffer);
	glBindBuffer(GL_ARRAY_BUFFER, buffer);
	//wrong
	//glBufferData(GL_ARRAY_BUFFER, sizeof(modelMatrix), &modelMatrix[0], GL_STATIC_DRAW);
	glBufferData(GL_ARRAY_BUFFER, num * sizeof(glm::mat4), modelMatrix, GL_STATIC_DRAW);
	for (int i = 0; i < rockModel.meshes.size(); i++)
	{
		unsigned int vao = rockModel.meshes[i].getVAO();
		glBindVertexArray(vao);
		glEnableVertexAttribArray(5);
		glEnableVertexAttribArray(6);
		glEnableVertexAttribArray(7);
		glEnableVertexAttribArray(8);
		glVertexAttribPointer(5, 4, GL_FLOAT, GL_FALSE, sizeof(glm::mat4), (void*)0);
		glVertexAttribPointer(6, 4, GL_FLOAT, GL_FALSE, sizeof(glm::mat4), (void*)(1*sizeof(glm::vec4)));
		glVertexAttribPointer(7, 4, GL_FLOAT, GL_FALSE, sizeof(glm::mat4), (void*)(2*sizeof(glm::vec4)));
		glVertexAttribPointer(8, 4, GL_FLOAT, GL_FALSE, sizeof(glm::mat4), (void*)(3*sizeof(glm::vec4)));
		glVertexAttribDivisor(5, 1);
		glVertexAttribDivisor(6, 1);
		glVertexAttribDivisor(7, 1);
		glVertexAttribDivisor(8, 1);
		glBindVertexArray(0);
	}

	while (!glfwWindowShouldClose(window))
	{
		float currentFrame = glfwGetTime();
		deltaTime = currentFrame - lastFrame;
		lastFrame = currentFrame;

		processInput(window);

		glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		glm::mat4 projection = glm::perspective(glm::radians(45.0f), float(SCR_WIDTH) / float(SCR_HEIGHT), 0.1f, 1000.0f);
		glm::mat4 view = camera.GetViewMatrix();

		planetShader.useProgram();
		planetShader.setMat4("projection", projection);
		planetShader.setMat4("view", view);
		glm::mat4 model = glm::mat4(1.0);
		model = glm::translate(model, glm::vec3(0.0, -3.0, 0.0));
		model = glm::scale(model, glm::vec3(4.0));
		planetShader.setMat4("model", model);
		planetModel.Draw(planetShader);

		rockShader.useProgram();
		rockShader.setMat4("projection", projection);
		rockShader.setMat4("view", view);
		rockShader.setInt("texture_diffuse1", 0);
		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, rockModel.textures_loaded[0].id);
		for (int i = 0; i < rockModel.meshes.size(); i++)
		{
			glBindVertexArray(rockModel.meshes[i].getVAO());
			glDrawElementsInstanced(GL_TRIANGLES, rockModel.meshes[i].indices.size(), GL_UNSIGNED_INT, 0, num);
		}

		glfwPollEvents();
		glfwSwapBuffers(window);
	}
	/*glDeleteVertexArrays(1, &vao);
	glDeleteBuffers(1, &vbo);
	glDeleteBuffers(1, &instanceVBO);*/

	glfwTerminate();
	return 0;
}


void framebuffer_size_callback(GLFWwindow* window, int width, int height)
{
	glViewport(0, 0, width, height);
}

void processInput(GLFWwindow* window)
{
	if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
	{
		glfwSetWindowShouldClose(window, true);
	}

	if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS)
		camera.ProcessKeyboard(FORWARD, deltaTime);
	if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS)
		camera.ProcessKeyboard(BACKWARD, deltaTime);
	if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS)
		camera.ProcessKeyboard(LEFT, deltaTime);
	if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS)
		camera.ProcessKeyboard(RIGHT, deltaTime);
}

void mouse_callback(GLFWwindow* window, double xpos, double ypos)
{
	if (firstMouse)
	{
		lastX = xpos;
		lastY = ypos;
		firstMouse = false;
	}

	float xoffset = xpos - lastX;
	float yoffset = lastY - ypos;

	lastX = xpos;
	lastY = ypos;

	camera.ProcessMouseMovement(xoffset, yoffset);
}

void scroll_callback(GLFWwindow* window, double xoffset, double yoffset)
{
	camera.ProcessMouseScroll(yoffset);
}
#include <glad/glad.h>


#include <iostream>
#include <string>
#include <vector>

#include <glm/glm.hpp>

#include <learnopengl/shader.h>

using namespace std;

struct Vertex
{
	glm::vec3 Position;
	glm::vec3 Normal;
	glm::vec2 TexCoords;
};

struct Texture
{
	unsigned int id;
	string type;
	string path;
};

struct Material
{
	glm::vec4 Ka;
	glm::vec4 Kd;
	glm::vec4 Ks;
};
class Mesh {
public:
	vector<Vertex> vertices;
	vector<unsigned int> indices;
	vector<Texture> textures;
	Material material;
	unsigned int uniformBlockIndex; //?????
	Mesh(vector<Vertex> vertices, vector<unsigned int> indices, vector<Texture> textures, Material material)
	{
		this->vertices = vertices;
		this->indices = indices;
		this->textures = textures;
		this->material = material;
		setMesh();
	}
	void Draw(Shader shader)
	{
		unsigned int diffuseN = 1;
		unsigned int specularN = 1;
		unsigned int normalN = 1;
		unsigned int heightN = 1;
		for (unsigned int i = 0; i < textures.size(); i++)
		{
			string n;
			string type = textures[i].type;
			if (type == "diffuseMap")
			{
				n = std::to_string(diffuseN++);
			}
			else if (type == "specularMap")
			{
				n = std::to_string(specularN++);
			}
			else if (type == "normalMap")
			{
				n = std::to_string(normalN++);
			}
			else if (type == "heightMap")
			{
				n = std::to_string(heightN++);
			}
			shader.useProgram();
			shader.setInt(type + n, i);
			glActiveTexture(0 + i);
			glBindTexture(GL_TEXTURE_2D, textures[i].id);
		}

		glBindVertexArray(vao);
		glBindBufferRange(GL_UNIFORM_BUFFER, 0, uniformBlockIndex, 0, sizeof(Material));//?????
		glDrawElements(GL_TRIANGLES, indices.size(), GL_UNSIGNED_INT, 0);
		glBindVertexArray(0); //unbind
		glActiveTexture(GL_TEXTURE0);
	}
private:
	unsigned int vao, vbo, ebo;
	void setMesh()
	{
		glGenVertexArrays(1, &vao);
		glGenBuffers(1, &vbo);
		glGenBuffers(1, &ebo);
		glGenBuffers(1, &uniformBlockIndex); //????????

		glBindVertexArray(vao);
		glBindBuffer(GL_ARRAY_BUFFER, vbo);
		glBufferData(GL_ARRAY_BUFFER, vertices.size() * sizeof(Vertex), &vertices[0], GL_STATIC_DRAW);
		glBindBuffer(GL_UNIFORM_BUFFER, uniformBlockIndex);//  ???????????
		glBufferData(GL_UNIFORM_BUFFER, sizeof(material), (void*)(&material), GL_STATIC_DRAW);//?????

		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo);
		glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.size() * sizeof(unsigned int), &indices[0], GL_STATIC_DRAW);

		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)0);
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, Normal));
		glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, TexCoords));
		glEnableVertexAttribArray(0);
		glEnableVertexAttribArray(1);
		glEnableVertexAttribArray(2);

		glBindVertexArray(0); //unbind
		glDeleteBuffers(1, &vbo);
		glDeleteBuffers(1, &ebo);
		
	}
};
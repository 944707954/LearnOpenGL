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
};

class Mesh {
public:
	vector<Vertex> vertices;
	vector<unsigned int> indices;
	vector<Texture> textures;
	Mesh(vector<Vertex> vertices, vector<unsigned int> indices, vector<Texture> textures)
	{
		this->vertices = vertices;
		this->indices = indices;
		this->textures = textures;
		setMesh();
	}
	void Draw(Shader shader)
	{
		unsigned int diffuseN = 1;
		unsigned int specularN = 1;
		for (unsigned int i = 0; i < textures.size(); i++)
		{
			string n;
			string type = textures[i].type;
			if (type == "diffuseMap")
			{
				n = std::to_string(diffuseN);
				diffuseN++;
			}
			else if (type == "specularMap")
			{
				n = std::to_string(specularN);
				specularN++;
			}
			shader.useProgram();
			shader.setInt(type + n, i);
			glActiveTexture(0 + i);
			glBindTexture(GL_TEXTURE_2D, textures[i].id);
		}

		glBindVertexArray(vao);
		glDrawElements(GL_TRIANGLES, indices.size(), GL_UNSIGNED_INT, 0);
		glBindVertexArray(0); //unbind
	}
private:
	unsigned int vao, vbo, ebo;
	void setMesh()
	{
		glGenVertexArrays(1, &vao);
		glGenBuffers(1, &vbo);
		glGenBuffers(1, &ebo);

		glBindVertexArray(vao);
		glBindBuffer(GL_ARRAY_BUFFER, vbo);
		glBufferData(GL_ARRAY_BUFFER, vertices.size() * sizeof(Vertex), &vertices[0], GL_STATIC_DRAW); 
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
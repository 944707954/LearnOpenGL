#pragma once
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
	glm::vec3 Tangent;
	glm::vec3 Bitangent;
};

struct Texture
{
	unsigned int id;
	string type;
	string path;
};

struct Material
{
	glm::vec3 Ambient;
	glm::vec3 Diffuse;
	glm::vec3 Specular;
	float Shininess;
};

class Mesh {
public:
	vector<Vertex> vertices;
	vector<unsigned int> indices;
	vector<Texture> textures;
	Material mat;
	Mesh(vector<Vertex> vertices, vector<unsigned int> indices, vector<Texture> textures, Material &mat)
	{
		this->vertices = vertices;
		this->indices = indices;
		this->textures = textures;
		this->mat = mat;
		setMesh();
	}
	void Draw(Shader shader)
	{
		unsigned int diffuseN = 1;
		unsigned int specularN = 1;
		unsigned int normalN = 1;
		unsigned int heightN = 1;
		unsigned int reflectN = 1;
		for (unsigned int i = 0; i < textures.size(); i++)
		{
			string n;
			string type = textures[i].type;
			if (type == "texture_diffuse")
			{
				n = std::to_string(diffuseN++);
			}
			else if (type == "texture_specular")
			{
				n = std::to_string(specularN++);
			}
			else if (type == "texture_normal")
			{
				n = std::to_string(normalN++);
			}
			else if (type == "texture_height")
			{
				n = std::to_string(heightN++);
			}
			else if (type == "texture_reflect")
			{
				n = std::to_string(reflectN++);
			}
			shader.useProgram();
			shader.setInt(type + n, i);
			//glActiveTexture(0 + i) wrong
			glActiveTexture(GL_TEXTURE0 + i);
			glBindTexture(GL_TEXTURE_2D, textures[i].id);
		}
		glBindVertexArray(vao);
		glDrawElements(GL_TRIANGLES, indices.size(), GL_UNSIGNED_INT, 0);
		glBindVertexArray(0); //unbind
		glActiveTexture(GL_TEXTURE0);

		//material
		shader.setVec3("material.Ambient", mat.Ambient);
		shader.setVec3("material.Diffuse", mat.Diffuse);
		shader.setVec3("material.Specular", mat.Specular);
		shader.setFloat("material.Shininess", mat.Shininess);
	}
	unsigned int getVAO()
	{
		return vao;
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

		// set the vertex attribute pointers
		// vertex Positions
		glEnableVertexAttribArray(0);
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)0);
		// vertex normals
		glEnableVertexAttribArray(1);
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, Normal));
		// vertex texture coords
		glEnableVertexAttribArray(2);
		glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, TexCoords));
		// vertex tangent
		glEnableVertexAttribArray(3);
		glVertexAttribPointer(3, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, Tangent));
		// vertex bitangent
		glEnableVertexAttribArray(4);
		glVertexAttribPointer(4, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, Bitangent));
		
		glBindVertexArray(0); 
		glDeleteBuffers(1, &vbo);
		glDeleteBuffers(1, &ebo);
		
	}
};
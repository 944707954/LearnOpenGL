#pragma once
#include <glad/glad.h>
#define STB_IMAGE_IMPLEMENTATION
#include <stb_image.h>

#include <iostream>
#include <vector>

#include <learnopengl/mesh.h>

#include <assimp/Importer.hpp>
#include <assimp/scene.h>
#include <assimp/postprocess.h>

using namespace std;

unsigned int TextureFromFile(const char* path, const string& directory, bool gamma = false);

class Model {
public:
	vector<Texture> textures_loaded;
	vector<Mesh> meshes;
	string directory;
	bool gammaCorrection;

	Model(const char* path, bool gamma = false):gammaCorrection(gamma)
	{
		loadModel(path);
	}

	void Draw(Shader shader)
	{
		for (int i = 0; i < meshes.size(); i++)
		{
			meshes[i].Draw(shader);
		}
	}

private:
	
	void loadModel(string path)
	{
		Assimp::Importer importer;
		const aiScene* scene = importer.ReadFile(path, aiProcess_Triangulate | aiProcess_FlipUVs | aiProcess_CalcTangentSpace);
		if (!scene || !scene->mRootNode || scene->mFlags & AI_SCENE_FLAGS_INCOMPLETE)
		{
			cout << "error:assimp " << importer.GetErrorString() << endl;
			return;
		}
		directory = path.substr(0, path.find_last_of('/'));
		processNode(scene->mRootNode, scene);
	}

	void processNode(aiNode* node, const aiScene* scene)
	{
		//for all mesh
		for (int i = 0; i < node->mNumMeshes; i++)
		{
			aiMesh* mesh = scene->mMeshes[node->mMeshes[i]];
			meshes.push_back(processMesh(mesh, scene));
		}
		//for all node
		for (int i = 0; i < node->mNumChildren; i++)
		{
			processNode(node->mChildren[i], scene);
		}
	}

	Mesh processMesh(aiMesh* mesh, const aiScene* scene)
	{
		vector<Vertex> vertices;
		vector<unsigned int> indices;
		vector<Texture> textures;
		
		//vertices
		for (int i = 0; i < mesh->mNumVertices; i++)
		{
			Vertex vertex;
			glm::vec3 tmp;

			tmp.x = mesh->mVertices[i].x;
			tmp.y = mesh->mVertices[i].y;
			tmp.z = mesh->mVertices[i].z;
			vertex.Position = tmp;

			tmp.x = mesh->mNormals[i].x;
			tmp.y = mesh->mNormals[i].y;
			tmp.z = mesh->mNormals[i].z;
			vertex.Normal = tmp;

			if (mesh->mTextureCoords[0]) 
			{
				glm::vec2 tmp2;
				tmp2.x = mesh->mTextureCoords[0][i].x;
				tmp2.y = mesh->mTextureCoords[0][i].y;
				vertex.TexCoords = tmp2;
			}
			else
			{
				vertex.TexCoords = glm::vec2(0.0f, 0.0f);
			}
			tmp.x = mesh->mTangents[i].x;
			tmp.y = mesh->mTangents[i].y;
			tmp.z = mesh->mTangents[i].z;
			vertex.Tangent = tmp;

			tmp.x = mesh->mBitangents[i].x;
			tmp.y = mesh->mBitangents[i].y;
			tmp.z = mesh->mBitangents[i].z;
			vertex.Bitangent = tmp;

			vertices.push_back(vertex);
		}
		//indices
		for (int i = 0; i < mesh->mNumFaces; i++)
		{
			aiFace face = mesh->mFaces[i];
			for (int j = 0; j < face.mNumIndices; j++)
			{
				indices.push_back(face.mIndices[j]);
			}
		}
		//texture
		aiMaterial* material = scene->mMaterials[mesh->mMaterialIndex];
		
		//not use
		Material mat = loadMaterial(material);

		cout << "MaterialIndex: " << mesh->mMaterialIndex << endl;
		
		//if (mesh->mMaterialIndex >= 0){ }
		// 1. diffuse maps
		vector<Texture> diffuseMaps = loadMaterialTextures(material, aiTextureType_DIFFUSE, "texture_diffuse");
		textures.insert(textures.end(), diffuseMaps.begin(), diffuseMaps.end());
		// 2. specular maps
		vector<Texture> specularMaps = loadMaterialTextures(material, aiTextureType_SPECULAR, "texture_specular");
		textures.insert(textures.end(), specularMaps.begin(), specularMaps.end());
		//.obj need use aiTextureType_HEIGHT to load normalMaps
		// 3. normal maps
		std::vector<Texture> normalMaps = loadMaterialTextures(material, aiTextureType_HEIGHT, "texture_normal");
		textures.insert(textures.end(), normalMaps.begin(), normalMaps.end());
		// 4. height maps
		//std::vector<Texture> heightMaps = loadMaterialTextures(material, aiTextureType_AMBIENT, "texture_height");
		//textures.insert(textures.end(), heightMaps.begin(), heightMaps.end());
		// 5. reflect maps store in ambient
		std::vector<Texture> reflectMaps = loadMaterialTextures(material, aiTextureType_AMBIENT, "texture_reflect");
		textures.insert(textures.end(), reflectMaps.begin(), reflectMaps.end());
		cout << "mesh: " << endl;
		cout << "vertices size: " << vertices.size() << endl;
		cout << "indices size: " << indices.size() << endl;
		cout << "texture size: " << textures.size() << endl << endl;
		
		return Mesh(vertices, indices, textures, mat);
	}

	Material loadMaterial(aiMaterial *mat)
	{
		Material material;
		aiColor3D color(0.0f, 0.0f, 0.0f);
		float shininess;
		mat->Get(AI_MATKEY_COLOR_AMBIENT, color);
		material.Ambient = glm::vec3(color.r, color.g, color.b);
		mat->Get(AI_MATKEY_COLOR_DIFFUSE, color);
		material.Diffuse = glm::vec3(color.r, color.g, color.b);
		mat->Get(AI_MATKEY_COLOR_SPECULAR, color);
		material.Specular = glm::vec3(color.r, color.g, color.b);
		mat->Get(AI_MATKEY_SHININESS, shininess);
		material.Shininess = shininess;
		return material;
	}
	vector<Texture> loadMaterialTextures(aiMaterial* material, aiTextureType type, string typeName)
	{
		vector<Texture> textures;
		//get the num of texture, but num = 0
		cout << "texture_" << typeName << ": " << material->GetTextureCount(type) << endl;
		for (unsigned int i = 0; i < material->GetTextureCount(type); i++)
		{
			
			aiString str;
			material->GetTexture(type, i, &str);
			cout << str.C_Str() << endl;
			bool skip = false;
			for (int j = 0; j < textures_loaded.size(); j++)
			{
				if (strcmp(textures_loaded[j].path.c_str(), str.C_Str()) == 0)
				{
					textures.push_back(textures_loaded[j]);
					skip = true;
					break;
				}
			}
			if (!skip)
			{
				Texture texture;
				texture.id = TextureFromFile(str.C_Str(), directory);
				texture.type = typeName;
				texture.path = str.C_Str();
				textures.push_back(texture);
				textures_loaded.push_back(texture);

			}
		}
		return textures;
	}
	
};


unsigned int TextureFromFile(const char* path, const string& directory, bool gamma)
{
	string filename = string(path);
	filename = directory + '/' + filename;
	unsigned int textureID;
	glGenTextures(1, &textureID);

	int width, height, nrComponents;
	unsigned char* data = stbi_load(filename.c_str(), &width, &height, &nrComponents, 0);
	if (data)
	{
		GLenum format;
		if (nrComponents == 1)
		{
			format = GL_RED;
		}
		else if (nrComponents == 3)
		{
			format = GL_RGB;
			cout << "format = GL_RGB" << endl;
		}
		else if (nrComponents == 4)
		{
			format = GL_RGBA;
			cout << "format = GL_RGBA" << endl;
		}
		
		glBindTexture(GL_TEXTURE_2D, textureID);
		glTexImage2D(GL_TEXTURE_2D, 0, format, width, height, 0, format, GL_UNSIGNED_BYTE, data);
		glGenerateMipmap(GL_TEXTURE_2D);

		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

		stbi_image_free(data);
	}
	else
	{
		std::cout << "Texture failed to load at path: " << path << std::endl;
		stbi_image_free(data);
	}

	return textureID;
}
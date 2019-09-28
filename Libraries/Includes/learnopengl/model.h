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

unsigned int TextureFromFile(const char* path, const string& directory);


class Model {
public:
	Model(const char* path)
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
	vector<Mesh> meshes;
	string directory;
	void loadModel(string path)
	{
		Assimp::Importer importer;
		const aiScene* scene = importer.ReadFile(path, aiProcess_Triangulate | aiProcess_FlipUVs);
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

			if (mesh->mTextureCoords[0]) //only include ��һ����������
			{
				glm::vec2 tmp2;
				tmp.x = mesh->mTextureCoords[0][i].x;
				tmp.y = mesh->mTextureCoords[0][i].y;
				vertex.TexCoords = tmp;

			}
			else
			{
				vertex.TexCoords = glm::vec2(0.0f, 0.0f);
			}

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
		if (mesh->mMaterialIndex >= 0)
		{
			aiMaterial* material = scene->mMaterials[mesh->mMaterialIndex];
			vector<Texture> diffuseMaps = loadMaterialTextures(material, aiTextureType_DIFFUSE, "diffuseMap");
			textures.insert(textures.end(), diffuseMaps.begin(), diffuseMaps.end());
			vector<Texture> specularMaps = loadMaterialTextures(material, aiTextureType_SPECULAR, "specularMap");
			textures.insert(textures.end(), specularMaps.begin(), specularMaps.end());
		}

		return Mesh(vertices, indices, textures);
	}

	vector<Texture> loadMaterialTextures(aiMaterial* material, aiTextureType type, string typeName)
	{
		vector<Texture> textures;
		for (unsigned int i = 0; i < material->GetTextureCount(type); i++)
		{
			aiString str;
			material->GetTexture(type, i, &str);
			Texture texture;
			texture.id = TextureFromFile(str.C_Str(), directory);
			texture.type = typeName;
			//texture.path = str;
			textures.push_back(texture);
			//// check if texture was loaded before and if so, continue to next iteration: skip loading a new texture
			//bool skip = false;
			//for (unsigned int j = 0; j < textures_loaded.size(); j++)
			//{
			//	if (std::strcmp(textures_loaded[j].path.data(), str.C_Str()) == 0)
			//	{
			//		textures.push_back(textures_loaded[j]);
			//		skip = true; // a texture with the same filepath has already been loaded, continue to next one. (optimization)
			//		break;
			//	}
			//}
			//if (!skip)
			//{   // if texture hasn't been loaded already, load it
			//	Texture texture;
			//	texture.id = TextureFromFile(str.C_Str(), this->directory);
			//	texture.type = typeName;
			//	texture.path = str.C_Str();
			//	textures.push_back(texture);
			//	textures_loaded.push_back(texture);  // store it as texture loaded for entire model, to ensure we won't unnecesery load duplicate textures.
			//}
		}
		return textures;
	}
};


unsigned int TextureFromFile(const char* path, const string& directory)
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
			format = GL_RED;
		else if (nrComponents == 3)
			format = GL_RGB;
		else if (nrComponents == 4)
			format = GL_RGBA;

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
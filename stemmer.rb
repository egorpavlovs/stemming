require 'rubygems'
require 'lingua/stemmer'
require 'fileutils'

DOCS_HOME_PATH = 'docs'
S_DOCS_HOME_PATH = 'stemming_docs'

class Stemmer
  class << self
    def all_docs
      all_paths = Dir["#{DOCS_HOME_PATH}/*/*/"]
      words = all_paths.map do |path|
        content = File.open([path, "content.txt"].join("/"), "r"){ |f| f.read }
        tokenize_words = tokenize(content).map { |word| Lingua.stemmer(word) unless word.nil? }.compact
        s_path = FileUtils.mkdir_p([S_DOCS_HOME_PATH, path.split("/").drop(1)].flatten.join("/"))
        puts s_path
        tokenize_words.each{ |word|
          File.open([s_path, "content.txt"].flatten.join("/"), "a") {|file| file.puts(word)}
        }
      end
    end

    def tokenize(content)
      numbers = content.scan(/\d+/)
      result = content.split(/\W+/) - numbers
      result.map{ |word| word if word.size > 2 && word.gsub(/\d+/,"") == word}
    end

  end
end

s = Stemmer.all_docs
puts "All stemming! Check #{S_DOCS_HOME_PATH}/"